using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Efemerides
{
    /// <summary>
    /// Objeto que representa una Efeméride    
    /// </summary>
    public class Efemeride
    {
        public string Texto { get; internal set; }
        public Tipo Tipo { get; internal set; }
        public DateTime Fecha { get; internal set; }
        ///
        public int RealYear { get; internal set; }

        public override string ToString()
        {
            return $"Efemeride => {this.Fecha.ToString("D").Replace(this.Fecha.Year.ToString(), this.RealYear.ToString())}, {this.Texto}";
        }

    }

    /// <summary>
    ///  Colección de efémerides 
    ///  Consta de :
    ///  Una instanciación de la API de efemérides.
    ///  Un constructor de la clase sin parametro donde se obtienen las efemerides del dia.
    ///  Un constructorde la clase con parametro donde se obtienen las efemerides del mes.
    ///  Un método que parsea las efemerides a JSON.
    ///  Un método que guarda en un archivo las efemerides convertidas a JSOn y devuelve la ruta del archivo.
    /// </summary>
    public class EfemerideCollection : List<Efemeride>
    {
        private Engine api;
        private DateTime instanceTime;
        public EfemerideCollection()
        {
            api = new Engine();
            instanceTime = DateTime.Today;
            this.AddRange(api.GetToday());
        }

        public EfemerideCollection(DateTime dateTime)
        {
            api = new Engine();
            instanceTime = dateTime;
            this.AddRange(api.ByDate(instanceTime));
        }

        protected EfemerideCollection(List<Efemeride> efemerides, DateTime date) : base(efemerides)
        {
            api = new Engine();
            instanceTime = date;
        }

        public EfemerideCollection Today()
        {
            return new EfemerideCollection(this.Where((t) => t.Fecha.Day == this.instanceTime.Day).ToList(), instanceTime);
        }

        public EfemerideCollection Year()
        {
            return new EfemerideCollection(this.Where((t) => t.Fecha.Year == this.instanceTime.Year).ToList(), instanceTime);
        }

        public string ToJson() => new JsonWrapper<EfemerideCollection>(this).ToString();

        public string ToFile(string filePath) => new JsonWrapper<EfemerideCollection>(this, filePath).Save();

    }

    /// <summary>
    /// Tipo de efeméride a obtener 
    /// </summary>
    public enum Tipo
    {
        Muerte,
        Nacimiento,
        Evento
    }
}

