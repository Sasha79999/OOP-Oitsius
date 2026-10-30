using System;
using Xunit;
using Lab12V15;

namespace Lab12V15.Tests
{
    public class StudentRepositoryTests
    {
        private Student CreateStudent(int id, string firstName)
        {
            return new Student(id, firstName, "Тестовий", new DateTime(2024, 9, 1));
        }

        private int CountAll(InMemoryRepository<Student> repository)
        {
            int count = 0;
            foreach (Student student in repository.GetAll())
            {
                count++;
            }
            return count;
        }

        [Fact]
        public void Add_ShouldAddStudent()
        {
            InMemoryRepository<Student> repository = new InMemoryRepository<Student>();

            repository.Add(CreateStudent(1, "Олена"));

            Assert.Equal(1, CountAll(repository));
        }

        [Fact]
        public void GetById_ShouldReturnStudent()
        {
            InMemoryRepository<Student> repository = new InMemoryRepository<Student>();
            repository.Add(CreateStudent(1, "Олена"));

            Student result = repository.GetById(1);

            Assert.Equal("Олена", result.FirstName);
        }

        [Fact]
        public void GetById_ShouldThrow_WhenNotFound()
        {
            InMemoryRepository<Student> repository = new InMemoryRepository<Student>();

            Assert.Throws<ArgumentException>(() => repository.GetById(99));
        }

        [Fact]
        public void GetAll_ShouldReturnAllStudents()
        {
            InMemoryRepository<Student> repository = new InMemoryRepository<Student>();
            repository.Add(CreateStudent(1, "Олена"));
            repository.Add(CreateStudent(2, "Іван"));

            Assert.Equal(2, CountAll(repository));
        }

        [Fact]
        public void GetAll_ShouldReturnNothing_WhenEmpty()
        {
            InMemoryRepository<Student> repository = new InMemoryRepository<Student>();

            Assert.Equal(0, CountAll(repository));
        }

        [Fact]
        public void Update_ShouldChangeStudent()
        {
            InMemoryRepository<Student> repository = new InMemoryRepository<Student>();
            repository.Add(CreateStudent(1, "Олена"));

            repository.Update(CreateStudent(1, "Петро"));

            Assert.Equal("Петро", repository.GetById(1).FirstName);
        }

        [Fact]
        public void Update_ShouldThrow_WhenNotFound()
        {
            InMemoryRepository<Student> repository = new InMemoryRepository<Student>();

            Assert.Throws<ArgumentException>(() => repository.Update(CreateStudent(99, "Петро")));
        }

        [Fact]
        public void Delete_ShouldRemoveStudent()
        {
            InMemoryRepository<Student> repository = new InMemoryRepository<Student>();
            repository.Add(CreateStudent(1, "Олена"));

            repository.Delete(1);

            Assert.Equal(0, CountAll(repository));
        }

        [Fact]
        public void Delete_ShouldThrow_WhenNotFound()
        {
            InMemoryRepository<Student> repository = new InMemoryRepository<Student>();

            Assert.Throws<ArgumentException>(() => repository.Delete(99));
        }
    }
}