using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using NavisBOQ.Revit.Plugin.Infrastructure;

namespace NavisBOQ.Revit.Plugin.Automation
{
    public static class BridgePoller
    {
        private static DateTime _lastPollUtc = DateTime.MinValue;
        private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(800);

        public static void OnIdling(object sender, IdlingEventArgs e)
        {
            try
            {
                if (DateTime.UtcNow - _lastPollUtc < PollInterval)
                    return;

                _lastPollUtc = DateTime.UtcNow;

                if (RevitBridgeState.IsProcessing)
                    return;

                if (RevitBridgeState.BridgeExternalEvent == null || RevitBridgeState.BridgeHandler == null)
                    return;

                if (!File.Exists(BridgePaths.RequestFile))
                    return;

                string requestJson = File.ReadAllText(BridgePaths.RequestFile, Encoding.UTF8);
                if (string.IsNullOrWhiteSpace(requestJson))
                    return;

                string fingerprint = ComputeSha1(requestJson);

                if (string.Equals(
                    RevitBridgeState.LastRequestFingerprint,
                    fingerprint,
                    StringComparison.OrdinalIgnoreCase))
                {
                    if (File.Exists(BridgePaths.ResponseFile))
                        return;
                }

                if (RevitBridgeState.BridgeExternalEvent.IsPending)
                    return;

                RevitBridgeState.IsProcessing = true;
                RevitBridgeState.BridgeExternalEvent.Raise();
            }
            catch
            {
                RevitBridgeState.IsProcessing = false;
            }
        }

        private static string ComputeSha1(string text)
        {
            using (var sha1 = SHA1.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text ?? "");
                byte[] hash = sha1.ComputeHash(bytes);
                var sb = new StringBuilder();

                for (int i = 0; i < hash.Length; i++)
                    sb.Append(hash[i].ToString("x2"));

                return sb.ToString();
            }
        }
    }
}