using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using Packt.Shared;
using static System.Console;
using static System.Environment;
using static System.IO.Path;
using System.Threading.Tasks;
// import new json class as alias to avoid conflicts with other json.NET names
using NuJson = System.Text.Json.JsonSerializer;

namespace WorkingWithSerialization
{
    class Program
    {
        static async Task Main(string[] args)
        {

            //create an object graph
            var people = new List<Person>
            {
                new Person(30000M) {
                    FirstName = "Alice",
                    LastName = "Smith",
                    DateOfBirth = new DateTime(1974, 3, 14)
                },
                new Person(40000M) {
                    FirstName = "Bob",
                    LastName = "Jones",
                    DateOfBirth = new DateTime(1969, 11, 23)
                },
                new Person(20000M) {
                    FirstName = "Charlie",
                    LastName = "Cox",
                    DateOfBirth = new DateTime(1984, 5, 4),
                    Children = new HashSet<Person>
                    {
                        new Person(0M)
                        {
                            FirstName = "Sally",
                            LastName = "Cox",
                            DateOfBirth = new DateTime(2000, 7, 12)
                        }
                    }
                }
            };

            // create an object that will format a list of persons as XML
            var xs = new XmlSerializer(typeof(List<Person>));

            //create a file to write to
            string path = Combine(CurrentDirectory, "people.xml");

            using (FileStream stream = File.Create(path))
            {
                //serialize the object graph to the stream
                xs.Serialize(stream, people);
            }

            WriteLine($"Written {new FileInfo(path).Length:N0} bytes of XML to {path}");
            WriteLine();

            //Display the serialized object graph
            WriteLine(File.ReadAllText(path));

            using (FileStream xmlLoad = File.Open(path, FileMode.Open))
            {
                //deserialize and cast the object graph into a list of Person
                var loadedPeople = (List<Person>)xs.Deserialize(xmlLoad);

                foreach (var item in loadedPeople)
                {
                    WriteLine($"{item.FirstName} {item.LastName} has {item.Children.Count} children.");
                }
            }

            WriteLine("Using JSON");
            WriteLine();
            //create a file to write to
            string jsonPath = Combine(CurrentDirectory, "people.json");

            using (StreamWriter jsonStream = File.CreateText(jsonPath))
            {
                //create an object that will format as JSON
                var jss = new Newtonsoft.Json.JsonSerializer();

                //serialize the object graph into a string
                jss.Serialize(jsonStream, people);
            }
            WriteLine();
            WriteLine($"Written {new FileInfo(jsonPath).Length:N0} bytes of JSON to: {jsonPath}");

            //Display the serialized object graph
            WriteLine(File.ReadAllText(jsonPath));

            using (FileStream jsonLoad = File.Open(jsonPath, FileMode.Open))
            {
                //deserialize object graph into a list of Person
                var loadedPeople = (List<Person>)

                    await NuJson.DeserializeAsync(
                        utf8Json: jsonLoad,
                        returnType: typeof(List<Person>));

                foreach(var item in loadedPeople)
                {
                    WriteLine($"{item.LastName} has {item.Children?.Count ?? 0} children.");
                }
            }
        }
    }
}
