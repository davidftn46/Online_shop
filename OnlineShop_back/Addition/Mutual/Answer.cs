using Addition.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addition.Mutual
{
    public class Answer<T>
    {
        public Feedback Status { get; set; } = Feedback.OK;

        public string Message { get; set; }

        public T Data { get; set; }

        public Answer()
        {
            Status = Feedback.OK;
            Message = string.Empty;
        }

        public Answer(T data, Feedback status = Feedback.OK, string message = "")
        {
            Data = data;
            Status = status;
            Message = message;
        }

        public Answer(Feedback status, string message)
        {
            Status = status;
            Message = message;
        }
    }

    public class AnswerNoData
    {
        public Feedback Status { get; set; } = Feedback.OK;

        public string Message { get; set; }

        public AnswerNoData()
        {
            Status = Feedback.OK;
            Message = string.Empty;
        }

        public AnswerNoData(Feedback status, string message)
        {
            Status = status;
            Message = message;
        }
    }
}
