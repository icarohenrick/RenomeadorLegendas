using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace RenomeadorLegenda
{
    
    class Program
    {
        public static string Caminho = string.Empty;
        public static string extensao = string.Empty;
        public static string palavra = string.Empty;
        public static string subsWord = string.Empty;
        public static int Opcao = 0;

        static void Main(string[] args)
        {
            OpçoesIniciais();
        }

        private static void OpçoesIniciais()
        {
            Console.WriteLine("Escolha as Opções:");
            Console.WriteLine("1 - Renomear Legendas");
            Console.WriteLine("2 - Substituir Palavras");
            Opcao = int.Parse(Console.ReadLine());

            if (Opcao == 1)
            {
                Console.WriteLine("Entre com o Caminho dos Episódios:");
                Caminho = Console.ReadLine().ToString();

                Console.WriteLine("Entre com extensao desejada:");
                extensao = Console.ReadLine();

                LerDiretorio(Caminho);

                Console.WriteLine("Finalizado!");

                OpçoesIniciais();
            }
            else if (Opcao == 2)
            {
                Console.WriteLine("Entre com o Caminho dos Episódios:");
                Caminho = Console.ReadLine().ToString();

                Console.WriteLine("Entre com extensao desejada:");
                extensao = Console.ReadLine();

                Console.WriteLine("Entre palavra:");
                palavra = Console.ReadLine();

                Console.WriteLine("Entre substituição:");
                subsWord = Console.ReadLine();

                LerDiretorio(Caminho);

                Console.WriteLine("Finalizado!");

                OpçoesIniciais();
            }
            else
            {
                Console.WriteLine("Opção invalida");
                OpçoesIniciais();
            }
        }

        private static void LerDiretorio(string path)
        {
            if (Directory.Exists(path))
            {
                ProcessDirectory(path);
            }
            else
            {
                Console.WriteLine("{0} is not a valid file or directory.", path);
            }
        }

        public static void ProcessDirectory(string targetDirectory)
        {
            var ListArquivos = new List<string>();

            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                if (Opcao == 1)
                {
                    ListArquivos.Add(fileName);
                }
                else if (Opcao == 2)
                {
                    ProcessFile(fileName);
                }
            }

            if(Opcao == 1)
            {
                if(ListArquivos.Any())
                {
                    var listMovie = ListArquivos.Where(x => x.EndsWith(extensao));
                    if(listMovie.Any())
                    {
                        foreach(var item in listMovie)
                        {
                            var FileName = Path.GetFileName(item);

                            var nomeArquivo = FileName.Split('.');

                            string nomeCompleto = string.Empty;

                            for(int i = 0; i < nomeArquivo.Count(); i++)
                            {
                                if(string.IsNullOrEmpty(nomeCompleto))
                                    nomeCompleto += nomeArquivo[i];
                                else
                                    nomeCompleto += $".{nomeArquivo[i]}";
                                var LegendaArquivo = ListArquivos.Where(x => x.EndsWith(".srt") && x.Contains(nomeCompleto)).ToList();
                                if (LegendaArquivo.Count == 1)
                                {
                                    Console.WriteLine($"Encontrado Arquivo: {LegendaArquivo.First()}");

                                    File.Move(LegendaArquivo.First(), item.Replace(extensao, ".srt"));
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }

        public static void ProcessFile(string path)
        {
            try
            {
                if (Path.GetExtension(path).EndsWith(extensao))
                {
                    string Texto = File.ReadAllText(path);

                    var finderWord = palavra.Split(',');
                    var fileReplace = subsWord.Split(',');

                    for(int i = 0; i < finderWord.Count(); i++)
                    {
                        Texto = Texto.Replace($"{finderWord[i]}", $"{fileReplace[i]}");
                    }

                    byte[] bytes = Encoding.ASCII.GetBytes(Texto);
                    Stream s = new MemoryStream(bytes);

                    using (StreamWriter writetext = new StreamWriter(path))
                    {
                        writetext.Write(Texto);
                    }

                    Console.WriteLine("deu certo '{0}'.", Path.GetFileName(path));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Processed file with error'{0}'.", Path.GetFileName(path));
            }
        }
    }
}
