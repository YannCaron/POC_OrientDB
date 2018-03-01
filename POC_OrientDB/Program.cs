using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RegularExtensions;
using Orient.Client;

namespace POC_OrientDB
{
    // Run console at http://localhost:2480

    class MainClass
    {

        private static string RecordToString(List<ODocument> record)
        {
            StringBuilder sb = new StringBuilder();
            ISet<string> headers = new SortedSet<string>();

            foreach (ODocument doc in record)
            {

                bool first = true;
                foreach (string key in doc.Keys)
                {
                    if (!(key.Contains("@") || key.Match(@"in_.*") || key.Match(@"out_.*")))
                    {
                        if (!first) sb.Append(", ");
                        first = false;
                        sb.Append(doc.GetField<object>(key));

                        headers.Add(key);
                    }
                }
                sb.Append("\n");

            }

            string header = String.Format("# {0}\n", headers.Aggregate((string arg1, string arg2) => arg1 + ", " + arg2));
            sb.Insert(0, header);

            return sb.ToString();
        }

        private static void executeQuery(ODatabase database, string query)
        {
            List<ODocument> result = database.Query(query);
            Console.WriteLine(String.Format("Query : {0}", query));
            Console.WriteLine(String.Format("Result [{0}]:\n{1}", result.Count, RecordToString(result)));
        }

        private static readonly string SERVER_NAME = "localhost";
        private static readonly int SERVER_PORT = 2424;
        private static readonly string USER = "root";
        private static readonly string PASSWORD = "root";
        private static readonly string DATABASE_NAME = "test";

        public static void Main(string[] args)
        {
            OServer server = new OServer(SERVER_NAME, SERVER_PORT, USER, PASSWORD);
            Console.WriteLine(String.Format("Test database exists ?  [{0}].", server.DatabaseExist(DATABASE_NAME, OStorageType.PLocal)));

            if (server.DatabaseExist(DATABASE_NAME, OStorageType.PLocal))
            {
                ODatabase database = new ODatabase(SERVER_NAME, SERVER_PORT, DATABASE_NAME, ODatabaseType.Graph, USER, PASSWORD);

                // get all ATSEP
                executeQuery(database, "SELECT * FROM ATSEP");

                // récupère tous les équipements qui sont dépendant de ceux donc picci est responçable
                executeQuery(database, "SELECT * FROM Equipment WHERE @rid in (" +
                             "SELECT BOTH('Responsible').out ('Depends') FROM ATSEP WHERE firstname = 'Picci'" +
                             ")");

            }

            Console.WriteLine("Success !");
        }
    }
}
