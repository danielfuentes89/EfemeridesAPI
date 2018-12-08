using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efemerides
{
    internal sealed class Engine
    {
        /// <summary>
        /// Establecemos la propiedad de HttpClient
        /// </summary>
        private HttpClient client;

        internal Engine()
        {
            // Hacemos una isntancia en el contructor para poder 
            // acceder a ella sin necesidad de crear varias instancias.
            this.client = new HttpClient();
        }

        /// <summary>
        ///  Obtenemos la fecha actual con un determinado formato
        /// </summary>
        /// <returns> Today.ToString("f") </returns>
        internal string GetCurrentDate()
        {
            return DateTime.Today.ToString("f");
        }

        /// <summary>
        /// Obtenemos las urls correspondientes a cada mes  
        /// </summary>
        /// <returns></returns>
        internal List<Efemeride> GetToday()
        {
            string monthName = DateTime.Now.ToString("MMMM");
            string urlMonth = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(monthName);
            return DownloadData("https://es.wikipedia.org/wiki/Wikipedia:Efemérides/" + urlMonth);
        }

        /// <summary>
        /// Obtenemos las urls correspondientes al mes del objeto DateTime
        /// </summary>
        /// <returns></returns>
        internal List<Efemeride> ByDate(DateTime dateTime)
        {
            string urlMonth = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dateTime.ToString("MMMM"));
            return DownloadData("https://es.wikipedia.org/wiki/Wikipedia:Efemérides/" + urlMonth);
        }

        /// <summary>
        /// Descargamos el html pasado por parametro de la pagina que corresponde
        /// </summary>
        /// <param name="client"></param>
        /// <param name="Url"></param>
        /// <returns></returns>
        internal List<Efemeride> DownloadData(string Url)
        {
            var html_document = new HtmlDocument();
            // Obtenemos el html de la pagina obtenida
            var html = client.Get(Url);
            html_document.LoadHtml(html);
            return IterarCollection(html_document);
        }

        /// <summary>
        /// Recorremos el html seleccionado con los selectores los divs que necesitamos
        /// </summary>
        /// <param name="dict_efemerides"></param>
        /// <param name="html_document"></param>
        /// <param name="colecctionFechas"></param>
        /// <returns></returns>
        private List<Efemeride> IterarCollection(HtmlDocument html_document)
        {
            List<Efemeride> efemerides = new List<Efemeride>();
            
            //Recorremos la lista con las efemerides y con .Zip unimos la fecha con la efemerides para guardarlas juntas
            foreach (var fecha_efemerides in html_document.DocumentNode.SelectNodes("//div[@class='mw-parser-output']/h2/span")
                .Zip(html_document.DocumentNode.SelectNodes("//div[@class='mw-parser-output']/ul"),
                Tuple.Create))
            {
                Efemeride efemeridesClass = new Efemeride();
                // Obtenemos la fecha
                efemeridesClass.Fecha = DateTime.Parse(fecha_efemerides.Item1.InnerText);
                // Obtenemos la efemeride
                efemeridesClass.Texto = fecha_efemerides.Item2.InnerText;
                // Añadimos a la lista fecha + efemerides
                efemerides.Add(efemeridesClass);

#if DEBUG
                Console.WriteLine(String.Format("{0}\n{1}", efemeridesClass.Fecha.ToString("M"), efemeridesClass.Texto));
#endif
            }

            return ProcessResults(efemerides);
        }

        /// <summary>
        /// Creamos un lista de efemerides y lo recorremos para separar Fechas de Efemerides
        /// y quitar espacios en blanco, texto sobrante etc..
        /// </summary>
        /// <param name="dict_efemerides"></param>
        /// <returns></returns>
        internal List<Efemeride> ProcessResults(List<Efemeride> dict_efemerides)
        {
            string[] CacheEfemeride = new string[10];
            // Recorremos la lista total de las efemerides 
            List<Efemeride> processed = new List<Efemeride>();
            foreach (var item in dict_efemerides)
            {

                // Si hay alguna string con el texto '(en la imagen)' lo eliminamos
                if (item.Texto.Trim().Replace(".—", "-").Contains("(en la imagen)"))
                    CacheEfemeride = item.Texto.Trim().Replace(".—", "-").Replace("(en la imagen)", " ").Split('\n');
                else
                    CacheEfemeride = item.Texto.Trim().Replace(".—", "-").Split('\n');


                // Si hay alguna string cque contenga a.C o D.C lo eliminamos. 
                if (item.Texto.Trim().Replace(".—", "-").Contains("a. C"))
                    CacheEfemeride = item.Texto.Trim().Replace(".—", "-").Replace("a. C", "AC").Split('\n');
                else if (item.Texto.Trim().Replace(".—", "-").Contains("d. C"))
                    CacheEfemeride = item.Texto.Trim().Replace(".—", "-").Replace("d. C", "DC").Split('\n');

                // Recorremos las efemerides de cada dia, de forma individual, para insertarlas con este formato:
                // Efemeride, Mes, Dia
                foreach (var item_2 in CacheEfemeride)
                {
                    Efemeride efemeride = new Efemeride();
                    efemeride.Texto = item_2.Replace("&#160;", " ");

                    // Establecemos el tipo de efemeride segun lo que contenga la string 
                    if (efemeride.Texto.Contains("Nace")) efemeride.Tipo = Tipo.Nacimiento;
                    else if (efemeride.Texto.Contains("Muere")) efemeride.Tipo = Tipo.Muerte;
                    else efemeride.Tipo = Tipo.Evento;
                    // Quitamos guiones y los limpiamos la string de caracteres que no necesitamos.
                    efemeride.Fecha = item.Fecha;
                    var splitted = efemeride.Texto.Split('-').ToList();

                    string yearString = splitted[0];
                    efemeride.Texto = String.Join(",", splitted.Skip(1).ToList());

                    int realYear = 0;
                    int.TryParse(yearString, out realYear);

                    try
                    {
                        // Cogemos un la fecha de este año y la fecha obtenida de la efemeride y las restamos
                        efemeride.Fecha.AddYears((efemeride.Fecha.Year - realYear) * -1);
                    }
                    catch
                    { }
                    // Obtenemos el valor , del año de la efemeride ya calculado 
                    efemeride.RealYear = realYear;
#if DEBUG
                    if (efemeride.RealYear != efemeride.Fecha.Year) Console.WriteLine($"[!] Warning Parsed Year mismtach with RealYear {efemeride.Fecha.Year}<>{efemeride.RealYear}");
#endif

                    // Una vez procesada la efemeride la añadimos a la lista 
                    processed.Add(efemeride);
                }
            }
            return processed;
        }
    }

}
