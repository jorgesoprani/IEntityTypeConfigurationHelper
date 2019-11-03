using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEntityTypeConfigurationBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"C:\Git\ideation_2\src\Ideation.Persistance\Data\";
            var diretorioOutput = $"{path}\\Configuration";
            var filePath = $"{path}\\IdeationDbContext.cs";
            var nspaceConfigurations = @"Ideation.Persistance.Data.Configuration";
            var nspaceDomainEntities = @"Ideation.Domain.Entities";

            using (StreamReader file = new StreamReader(filePath))
            {
                int counter = 0;
                string ln;

                while ((ln = file.ReadLine()) != null)
                {
                    if (ln.Contains("modelBuilder.Entity<"))
                    {
                        var nomeEntidade = ln.Substring(ln.IndexOf("<") + 1, ln.IndexOf(">") - ln.IndexOf("<") - 1);

                        var configuration = new StringBuilder();
                        file.ReadLine();
                        while ((ln = file.ReadLine())?.EndsWith("});") == false)
                        {
                            configuration.AppendLine(ln);
                        }

                        if (!Directory.Exists(diretorioOutput))
                            Directory.CreateDirectory(diretorioOutput);

                        var output = Template
                            .Replace("@entidades", nspaceDomainEntities)
                            .Replace("@namespace", nspaceConfigurations)
                            .Replace("@entidade", nomeEntidade)
                            .Replace("@config", configuration.ToString());

                        File.WriteAllText($"{diretorioOutput}\\{nomeEntidade}Configuration.cs", output);
                    }

                    counter++;
                }
                file.Close();
                Console.WriteLine($"File has {counter} lines.");
            }
            Console.ReadKey();
        }

        public static string Template = @"using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using @entidades;

namespace @namespace
{
    public class @entidadeConfiguration : IEntityTypeConfiguration<@entidade>
    {
        public void Configure(EntityTypeBuilder<@entidade> builder)
        {
            @config
        }
    }
}";
    }
}
