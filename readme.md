# Efémerides API 
<br/>

## Introducción
 La libreria de efemérides sirve para obtener las efemémerides de un mes y  <i>filtrarse por día o año</i>
 
Todo el contenido de esta librería es obtenido de las efemerides alojadas en [Wikipedia(ES)](https://es.wikipedia.org/wiki/Wikipedia:Efemérides)

## Como se usa

Esta api tiene dos objetos, <b>Efemerides.EfemerideCollection</b> y <b>Efemerides.Efemeride</b>,
su uso principalemente se encuentra en el objeto <i>EfemerideCollection</i>, ya que <i>Efemeride</i>, 
describe el contenido de una efemeride.

``` C# 

namespace Efemerides{
    public class Efemeride{	
        public string Texto { get; internal set; }
        public Tipo Tipo { get; internal set; }
        public DateTime Fecha { get; internal set; }

        public int RealYear { get; internal set; }		 
	...
```
Este <b>Efemeride</b> como peculiaridad tiene <i>RealYear</i>, que se ha creado con el fin de solventar un problema que 
encuentro a la hora de crear/actualizar un DateTime con fechas <b><i>inferiores al 1900</b></i>. 
<br/><b>RealYear</b>  contendrá siempre el año real obtenido de la efeméride

Hemos dividido las efemerides en tres tipos, definidos en el enum Tipo

```C#
namespace Efemerides
{
    public enum Tipo
    {
        Muerte,
        Nacimiento,
        Evento
    }
...
```

Donde toda aquella que no hemos catalogado es definida como <b> Evento</b>

EfemerideCollection se puede inicializar con 2 constructores, sin parametros o con un <b>DateTime</b>
```C#
public class EfemerideCollection {
	public EfemerideCollection() // Utilizará DateTime.Now;
	public EfemerideCollection(DateTime dateTime)
	...
```
la consulta a la página web se hará por <b>mes</b>, pero usará el valor del día para la function <i><b>EfemerideCollection().Today()</b></i>
como para el valor del año del objeto <b>DateTime</b>, el resto de propiedades de <b>DateTime</b> no se utilizan.
</br></br>
<b>EfemerideCollection</b> Es una implementación de <i><b>List<Efemeride></b></i> por lo que puedes utilizar el objeto como si de 
una lista se tratará, siendo compatible tambien con <b><i>Linq</i></b>


Durante el desarrollo consideré util la necesidad de exportar, y elegí el formato JSON, del mismo modo que simplifique
la escritura a ficheros JSON, con los metodos <b><i>ToJson();</i></b> y <b><i>ToFile(string filePath);</i></b>
```C#
public class EfemerideCollection {
	public string ToJson()
	public string ToFile(string filePath)
	...
```
<br/><br/>

## Ejemplos

Obtener una lista de efemerides para el día anterior.

``` C#
	EfemerideCollection efemerides = new EfemerideCollection(); //Cargamos las efemerides de la fecha del sistema
	date.AddDays(-1); // restamos un día para obtener las efemerides de ayer
	List<Efemeride> efemeridesAyer = new EfemerideCollection(date).toList(); // Inicializamos y carga las efemerides de ayer
```	
<br/>

 Obtener las efemerides de el día actual
``` C#
	EfemerideCollection efemerides = new EfemerideCollection(); //Cargamos las efemerides de la fecha del sistema
	EfemerideCollection efemeridesDia = efemeridesAyer.Today();
```
<br/>

Obtener las efemerides de enero de 1998
``` C#
	EfemerideCollection efemerides = new EfemerideCollection(new DateTime(1, 1, 1998));
	EfemerideCollection efemeridesYear = efemeridesAyer.Year();
``` 
<br/>

Guardamos las efemerides de hoy en un fichero JSON
``` C#
	new EfemerideCollection().Today().ToFile("todayefemerides.json");
```
