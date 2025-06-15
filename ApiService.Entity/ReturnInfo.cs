using System;

namespace ApiService.Entity
{
    public class ReturnInfo
    {
    }
    public class DbReturnInfo<Type>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public Type Result { get; set; }
    }

    public class ReturnBaseInfo<T>
    {
        public StatusBaseInfo ReturnStatus { get; set; }
        public T ReturnData { get; set; }
    }
    public class StatusBaseInfo
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
