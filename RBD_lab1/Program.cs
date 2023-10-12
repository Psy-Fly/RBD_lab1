using System.Globalization;
using System.Text;
using System.Text.Json;
using Aspose.Cells;
using Bogus;
using CsvHelper;
using CsvHelper.Configuration;
using Name = Bogus.DataSets.Name;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


UsersGenerator usersGenerator = new UsersGenerator();
var usersFirst = usersGenerator.GenerateFirst(1000);
var users = usersGenerator.GenerateSecond(ref usersFirst);


//Записываем в формате csv
var userPath = @"D:\Documents\RBD\users.csv";
WriteCsvUsers(users, userPath);

var publicationsPath = @"D:\Documents\RBD\publications.csv";
WriteCsvPublications(users, publicationsPath);

var reviewPath = @"D:\Documents\RBD\reviews.csv";

WriteCsvReviews(users, reviewPath);


//Считываем

List<User> newUsers;
using (var reader = new StreamReader(userPath))
{
    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
    {
        newUsers = csv.GetRecords<User>().ToList();
    }
}

List<Publication> newPublications;
using (var reader = new StreamReader(publicationsPath))
{
    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
    {
        newPublications = csv.GetRecords<Publication>().ToList();
    }
}

List<Review> newReviews;

using (var reader = new StreamReader(reviewPath))
{
    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
    {
        newReviews = csv.GetRecords<Review>().ToList();
    }
}


DeleteNonApprovedUsers();


var options = new JsonSerializerOptions { WriteIndented = true };

var userJsonPath = @"D:\Documents\RBD\usersJson.json";
var publicationJsonPath = @"D:\Documents\RBD\publicationsJson.json";
var reviewJsonPath = @"D:\Documents\RBD\reviewsJson.json";

var jsonUsers = JsonSerializer.Serialize(newUsers, options);
var jsonPublications = JsonSerializer.Serialize(newPublications, options);
var jsonReviews = JsonSerializer.Serialize(newReviews, options);

File.WriteAllText(userJsonPath, jsonUsers);
File.WriteAllText(publicationJsonPath, jsonPublications);
File.WriteAllText(reviewJsonPath, jsonReviews);


void WriteCsvUsers(List<User> users, string path)
{
    using (StreamWriter streamReader = new StreamWriter(path))
    {
        using (CsvWriter csvReader = new CsvWriter(streamReader, CultureInfo.InvariantCulture))
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
}

void WriteCsvPublications(List<User> users, string path)
{
    using (StreamWriter streamReader = new StreamWriter(path))
    {
        using (CsvWriter csvReader = new CsvWriter(streamReader, CultureInfo.InvariantCulture))
        {
            csvReader.WriteHeader<Publication>();
            csvReader.NextRecord();

            foreach (var user in users)
            {
                foreach (var publication in user.Publications)
                {
                    csvReader.WriteRecord(publication);
                    csvReader.NextRecord();
                }
            }
            
        }
    }
}

void WriteCsvReviews(List<User> users, string path)
{
    using (StreamWriter streamReader = new StreamWriter(path))
    {
        using (CsvWriter csvReader = new CsvWriter(streamReader, CultureInfo.InvariantCulture))
        {
            csvReader.WriteHeader<Review>();
            csvReader.NextRecord();

            foreach (var user in users)
            {
                foreach (var publication in user.Publications)
                {
                    foreach (var review in publication.Reviews)
                    {
                        csvReader.WriteRecord(review);
                        csvReader.NextRecord();
                    }
                }
            }
        }
    }
}

void DeleteNonApprovedUsers()
{
    for (int i = 0; i < newUsers.Count; i++)
    {
        if (!newUsers[i].IsApproved)
        {
            newReviews.RemoveAll(x => x.UserId == newUsers[i].Id);
            newPublications.RemoveAll(x => x.UserId == newPublications[i].Id);
            newUsers.Remove(newUsers[i]);
            i--;

        }
    }
}

void Print(List<User> users)
{
    for (int i = 0; i < users.Count; i++)
    {
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("Id: " + users[i].Id);
        Console.WriteLine("Имя: " + users[i].Name);
        Console.WriteLine("Пол: " + users[i].Gender);
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
                Console.WriteLine("Пользователь номер: " + review.UserId);
                Console.WriteLine(review.ReviewText);
            }
        }

        Console.WriteLine("-------------------------------------------");
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<string> Emails { get; set; }

    public string EmailsString
    {
        get { return string.Join(",", Emails); }
        set { Emails = value.Split(',').ToList(); }
    }

    public DateTime RegDate { get; set; }
    public DateTime LastAuth { get; set; }
    public bool IsApproved { get; set; }
    public List<Publication> Publications { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
}

public class Publication
{
    public int Id { get; set; }
    public int UserId { get; set; }
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
    public int SenderUserId { get; set; }
    public int PublicationId { get; set; }
    public string ReviewText { get; set; }
}

public class UsersGenerator
{
    public List<User> GenerateFirst(int usersCount)
    {
        Faker faker = new Faker("en");
        var users = new List<User>();

        for (int i = 0; i < usersCount; i++)
        {
            var user = new User();
            user.Id = i;
            var gender = faker.PickRandom(Name.Gender.Female, Name.Gender.Male);
            user.Gender = gender.ToString();
            user.Name = faker.Name.FirstName(gender);
            user.Emails = new List<string>();
            for (int j = 0; j < new Random().NextInt64(1, 4); j++)
            {
                user.Emails.Add(new Faker("en").Internet.Email(firstName: user.Name, uniqueSuffix: user.Id.ToString()));
            }

            user.RegDate = faker.Date.Past(faker.Random.Number(1, 5));
            user.BirthDate = faker.Date.Past(faker.Random.Number(20, 40));
            user.LastAuth = faker.Date.Recent();
            user.IsApproved = faker.Random.Number(0, 2) != 0;


            users.Add(user);
        }

        return users;
    }

    public List<User> GenerateSecond(ref List<User> users)
    {
        for (int i = 0; i < users.Count; i++)
        {
            Faker faker = new Faker("en_US");
            var user = users[i];

            user.Publications = new List<Publication>();
            for (int j = 0; j < faker.Random.Number(1, 3); j++)
            {
                var publication = new Publication();
                publication.Id = j;
                publication.UserId = i;
                publication.Name = faker.Commerce.Product();
                publication.PublicationDate = faker.Date.Past();
                publication.Category = faker.Commerce.Categories(1)[0];
                publication.Description = faker.Lorem.Paragraph(3);
                publication.Size = faker.Random.Number(1, 3);
                publication.Reviews = new List<Review>();
                for (int k = 0; k < faker.Random.Number(0, 3); k++)
                {
                    var review = new Review();
                    review.UserId = i;
                    review.SenderUserId = faker.Random.Number(0, users.Count);
                    review.PublicationId = j;
                    review.ReviewText = faker.Lorem.Paragraph(4);
                    publication.Reviews.Add(review);
                }

                user.Publications.Add(publication);
            }
        }

        return users;
    }
}