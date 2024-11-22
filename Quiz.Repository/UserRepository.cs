using Quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Quiz.Repository
{
    public class UserRepository
    {
        private readonly string _filePath;
        private readonly List<User> _users;
        public User MainUser;

        public UserRepository(string filePath, string name, string mail)
        {
            _filePath = filePath;
            _users = LoadData();
            foreach (var user in _users)
            {
                if (user.Name == name)
                {
                    MainUser = LoginUser(name);
                }
            }
            if (MainUser == null)
            {
                MainUser = RegUser(name, mail);
            }
        }
        public string GetInfo() => $"{MainUser.Name} with the {MainUser.Mail} mail has best score of {MainUser.BestScore}";

        private List<User> LoadData()
        {
            if (!File.Exists(_filePath))
                return new List<User>();

            string text = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<User>>(text);
        }
        private void SaveData()
        {
            string jsonString = JsonSerializer.Serialize(value: _users, options: new JsonSerializerOptions()
            {
                WriteIndented = true
            });

            File.WriteAllText(_filePath, jsonString);
        }

        private User LoginUser(string name)
        {
            name.ToLower();
            foreach (var user in _users)
            {
                if (user.Name == name) return user;
            }

            return null;
        }

        private User RegUser(string name, string mail)
        {
            name.ToLower();

            User user = new()
            {
                Name = name,
                Mail = mail,
                BestScore = 0,
                id = (byte)((byte)_users.Count + 1)
            };
            _users.Add(user);
            SaveData();
            return user;
        }

        public void CreateQuiz()
        {

        }


    }
}
