using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Models
{
    public class Quizz
    {
        public byte Id { get; set; } 
        public byte UserId { get; set; }
        public List<Question> QuestionList { get; set; }

    }
}
