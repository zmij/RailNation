using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;

namespace RailNation
{
    public class CommandBody
    {
        private string hash_;
        private List<object> parameters_ = new List<object>();

        public string checksum { get; set; }
        public int client { get { return 1; } }
        public List<object> parameters
        {
            get
            {
                return parameters_;
            }
            set
            {
                parameters_ = value;
                hash_ = "";
            }
        }

        public string hash
        {
            get
            {
                if (String.IsNullOrEmpty(hash_)) {
                    hash_ = parameters_.ToJson().MD5();
                }
                return hash_;
            }
        }

        public override string ToString()
        {
            return this.ToJson();
        }
    }
}
