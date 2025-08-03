using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEB_SYSTEM.Models
{
    public class InventoryModel
    {
        public class Campus
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int ID { get; set; }

            public string? CampusName { get; set; }
        }

        public class User
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
        }
        public class Student
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Student_Id { get; set; }
            public string? Student_Name { get; set; }
            public string? Address { get; set; }
            public string? Tel_No { get; set; }
            public string? Grade { get; set; }
            public string? Section { get; set; }
        }

    }
}
