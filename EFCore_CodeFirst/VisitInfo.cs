using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore_CodeFirst
{
    // DTO (сущность)
    public class Student
    {
        public Guid Id { get; init; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateOnly Birthday { get; set; }
        public List<Visitation>? Visitations { get; set; }
    }
    public class Visitation
    {
        public Guid Id { get; init; }
        public DateOnly Date { get; set; }
        public Guid StudentId { get; set; }
        public Student? Student { get; set; }
    }
}
