using System.Collections.Generic;

namespace AutoAppManagement.Models.Common
{
    public class FilterCondition
    {
        public FilterOperator op { get; set; }
        public LogicalOperator aop { get; set; } = LogicalOperator.AND;
        public string field { get; set; }
        public string value { get; set; }
        public List<FilterCondition> ors { get; set; } = new List<FilterCondition>();
    }

    public enum LogicalOperator
    {
        AND = 1,
        OR = 2
    }

    public enum FilterOperator
    {
        Equals = 1,
        NotEquals = 2,
        Contains = 3,
        StartsWith = 4,
        EndsWith = 5,
        GreaterThan = 6,
        GreaterThanOrEqual = 7,
        LessThan = 8,
        LessThanOrEqual = 9,
        GreaterThan_Date = 10,
        LessThan_Date = 11,
        LessThanOrEqual_Date = 12,
        GreaterThanOrEqual_Date = 13
    }
}
