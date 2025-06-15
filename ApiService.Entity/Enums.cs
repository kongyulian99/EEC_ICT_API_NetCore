using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ApiService.Entity
{
    public enum QuestionType
    {
        [Description("Multiple Choice")]
        MULTIPLE_CHOICE = 1,
        
        [Description("Fill in the Blank")]
        FILL_IN_THE_BLANK = 2,
        
        [Description("True/False")]
        TRUE_FALSE = 3
    }

    public enum Enum_DifficutyLevel
    {
        [Description("Easy")]
        EASY = 1,
        
        [Description("Medium")]
        MEDIUM = 2,
        
        [Description("Hard")]
        HARD = 3
    }
}
