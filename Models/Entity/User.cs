using System.Globalization;
using System.Security.Cryptography;

namespace StudentApp.Models.Entity
{
	public class User
	{

		//private Guid id;
		private string id;
		private string name;
		private string email;
		private string picture; //¿?
		private string description;
		private string linkedIn;
		private string professionalBackground;
		private string password;
		private bool isActive;
		private DateTime createdAt;
		private string registrationStatus;
		private string role;

		public User()
		{
		}

		public User(string name, string email, string password)
		{
			//id = Guid.NewGuid(); //Genera el id 
			this.id = Guid.NewGuid().ToString();
			this.name = name;
			this.email = email;
			this.password = password;
			isActive = false; //predefinido para el momento de crear el registro
			createdAt = DateTime.Now; //Le asigna la fecha actual
			this.registrationStatus = "pending";
			role = "Student";
		}

		public User(string id, string name, string email, string picture, string role)
		{
			//id = Guid.NewGuid(); //Genera el id 
			this.id = id;
			this.picture = picture;
			this.name = name;
			this.email = email;
			this.role = role;
		}

        public User(string id, string name)
        { // se usa para advisement
            this.id = id;
            this.name= name;
        }

        public string Id { get => id; set => id = value; }
		public string Name { get => name; set => name = value; }
		public string Email { get => email; set => email = value; }
		public string Picture { get => picture; set => picture = value; }
		public string Description { get => description; set => description = value; }
		public string LinkedIn { get => linkedIn; set => linkedIn = value; }
		public string ProfessionalBackground { get => professionalBackground; set => professionalBackground = value; }
		public string Password { get => password; set => password = value; }
		public bool IsActive { get => isActive; set => isActive = value; }
		public DateTime CreatedAt { get => createdAt; set => createdAt = value; }
		public string RegistrationStatus { get => registrationStatus; set => registrationStatus = value; }
		public string Role { get => role; set => role = value; }
	}
}
