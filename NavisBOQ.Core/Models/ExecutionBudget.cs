namespace NavisBOQ.Core.Models
{
    public class ExecutionBudget
    {
        public int GreenCandidateLimit { get; set; }
        public int YellowCandidateLimit { get; set; }
        public int MaxNodesToVisit { get; set; }
        public int MaxDetailRows { get; set; }
        public int TimeoutMs { get; set; }
    }

    public static class BudgetProfiles
    {
        public static ExecutionBudget Corrida1 => new ExecutionBudget
        {
            GreenCandidateLimit = 10000,
            YellowCandidateLimit = 25000,
            MaxNodesToVisit = 50000,
            MaxDetailRows = 6000,
            TimeoutMs = 90000
        };

        public static ExecutionBudget Corrida2 => new ExecutionBudget
        {
            GreenCandidateLimit = 10000,
            YellowCandidateLimit = 25000,
            MaxNodesToVisit = 50000,
            MaxDetailRows = 6000,
            TimeoutMs = 90000
        };

        public static ExecutionBudget Corrida3 => new ExecutionBudget
        {
            GreenCandidateLimit = 5000,
            YellowCandidateLimit = 12000,
            MaxNodesToVisit = 50000,
            MaxDetailRows = 5000,
            TimeoutMs = 120000
        };

        public static ExecutionBudget Corrida4 => new ExecutionBudget
        {
            GreenCandidateLimit = 2500,
            YellowCandidateLimit = 6000,
            MaxNodesToVisit = 25000,
            MaxDetailRows = 800,
            TimeoutMs = 90000
        };

        public static ExecutionBudget Corrida5 => new ExecutionBudget
        {
            GreenCandidateLimit = 2000,
            YellowCandidateLimit = 5000,
            MaxNodesToVisit = 20000,
            MaxDetailRows = 600,
            TimeoutMs = 90000
        };

        public static ExecutionBudget Corrida6 => new ExecutionBudget
        {
            GreenCandidateLimit = 800,
            YellowCandidateLimit = 2000,
            MaxNodesToVisit = 5000,
            MaxDetailRows = 1000,
            TimeoutMs = 60000
        };
    }
}