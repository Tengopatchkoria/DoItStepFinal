using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Models
{
    public class User
    {
        private string mail;

        public byte id { get; set; }
        public string Name { get; set; }
        public string Mail
        {
            get { return mail; }
            set
            {
                mail = value.Contains("@") ? value : throw new Exception("Probable typo in the input");
            }
        }
        public int BestScore { get; set; }

    }
}
