using System.Globalization;
using Aspose.Cells;
using Bogus;
using CsvHelper;



// var path = @"C:\Users\5047449\Documents\RiderProjects\RBD_lab1\RBD_lab1\csv";
//
//
// StringBuilder jsonString = new StringBuilder("[");
//
// for (int i = 0; i < 1000; i++)
// {
//     jsonString.Append("{\"id\":" + "\""+ i + "\"" + "},");
// }
//
// jsonString.Append("]");
//
// var jsonStr = jsonString.ToString();
// var csvPath = jsonStr.ToCsv(path);
// Console.WriteLine(csvPath);

UsersGenerator usersGenerator = new UsersGenerator();
var users = usersGenerator.Generate(3);

var path = @"D:\Documents\RBD\rbd.csv";

using (StreamWriter streamReader = new StreamWriter(path))
{
    using (CsvWriter csvReader = new CsvWriter(streamReader, CultureInfo.CurrentCulture))
    {
        csvReader.WriteHeader<User>();
        csvReader.NextRecord();
        foreach (var user in users)
        {
            csvReader.WriteRecord(user);
            csvReader.NextRecord();
        }
    }
}

Workbook book = new Workbook(path);
book.Save(@"D:\Documents\RBD\jsonRBD.json", SaveFormat.Json);

for (int i = 0; i < users.Count; i++)
{
    Console.WriteLine("-------------------------------------------");
    Console.WriteLine("Id: " + users[i].Id);
    Console.WriteLine("Имя: " + users[i].Name);
    Console.WriteLine("Пол: "+ users[i].Gender);
    Console.WriteLine("Дата рождения: " + users[i].BirthDate);
    Console.WriteLine("Статус подтверждения: " + users[i].IsApproved);
    Console.WriteLine("Был в сети: " + users[i].LastAuth);
    Console.WriteLine("Зарегистрирован: " + users[i].RegDate);
    Console.WriteLine("Email адреса: ");
    Console.WriteLine("*");
    for (int j = 0; j < users[i].Emails.Count; j++)
    {
        Console.WriteLine(users[i].Emails[j]);
    }
    Console.WriteLine("*");
    Console.WriteLine("Публикации: ");
    for (int j = 0; j < users[i].Publications.Count; j++)
    {
        var publication = users[i].Publications[j];
        Console.WriteLine($"Публикация №{j}:");
        Console.WriteLine(publication.Name);
        Console.WriteLine(publication.PublicationDate);
        Console.WriteLine(publication.Category);
        Console.WriteLine(publication.Size + "стр");
        Console.WriteLine("Отзывы: ");
        for (int k = 0; k < publication.Reviews.Count; k++)
        {
            var review = publication.Reviews[k];
            Console.WriteLine($"Отзыв №{k}:");
            Console.WriteLine(review.UserId);
            Console.WriteLine(review.ReviewText);
        }
    }
    
    Console.WriteLine("-------------------------------------------");
}



public class User
{
    public int Id {get; set; }
    public string Name { get; set; }
    public List<string> Emails { get; set; }
    public DateTime RegDate { get; set; }
    public DateTime LastAuth { get; set; }
    public bool IsApproved { get; set; }
    public List<Publication> Publications { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
}

public class Publication
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Size { get; set; }
    public string Category { get; set; }
    public DateTime PublicationDate { get; set; }
    public List<Review> Reviews { get; set; }
}

public class Review
{
    public int UserId { get; set; }
    public string ReviewText { get; set; }
}

public class UsersGenerator
{
   public List<User> Generate(int usersCount)
    {
        Faker faker = new Faker("ru");
        var users = new List<User>();

        for (int i = 0; i < usersCount; i++)
        {
            var user = new User();
            user.Id = i;
            user.Name = faker.Name.FirstName();
            user.Emails = new List<string>();
            for (int j = 0; j < new Random().NextInt64(1,4); j++)
            {
                user.Emails.Add(new Faker("ru").Internet.Email(firstName:user.Name, uniqueSuffix:user.Id.ToString()));
            }

            user.RegDate = faker.Date.Past(faker.Random.Number(1,5));
            user.BirthDate = faker.Date.Past(faker.Random.Number(20,40));
            user.LastAuth = faker.Date.Recent();
            user.IsApproved = faker.Random.Number(0, 2) != 0;
            user.Publications = new List<Publication>();
            for (int j = 0; j < faker.Random.Number(1,3); j++)
            {
                var publication = new Publication();
                publication.Name = faker.Commerce.Product();
                publication.PublicationDate = faker.Date.Past();
                publication.Category = faker.Commerce.Categories(1)[0];
                publication.Description = faker.Lorem.Paragraph(3);
                publication.Size = faker.Random.Number(1, 3);
                publication.Reviews = new List<Review>();
                for (int k = 0; k < faker.Random.Number(0,3); k++)
                {
                    var review = new Review();
                    review.UserId = faker.Random.Number(1, 3000);
                    review.ReviewText = faker.Lorem.Paragraph(4);
                    publication.Reviews.Add(review);
                }
                user.Publications.Add(publication);
            }

            user.Gender = faker.Person.Gender.ToString();
            users.Add(user);

        }
        return users;
    }
}