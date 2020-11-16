
# Option 1

var b = Map.To<B>(a);

Ulempen her er at det geniriske argument ikke har indflydelse på respons typen. Hvis vi skal mappe en type til flere forskellige typer kommer vi i problemer.

Dette kan også laves som en extension method, så det bliver:
var b = a.Map<B>();

# Option 2

B b = Map.From(a);

Her er problemet at der ikke kan eksistere mere end én mapning fra A da det hele er på en enkelt mapping klasse.

# Option 3

var b = Map.ToB(b);

Her er flere problemer. Typen kan ikke genereres før vi har skrevet den ud, så der er ingen intellisense. Derudover kan navnet være tvetydigt hvis flere klasser har samme navn. Og til sidst er der issuet med lange metodenavne for lange klassenavne.

En variant af denne er hvis vi bruger marker interfaces til at indikere hvilke klasser kan mappes til hvilke andre klasser. Med disse kan vi generere mapping metoderne up front. Hvis vi gør dette kan vi også genere en statisk mapping for både enkelte elementer og collections.

bar b = A.ToB(a);
var listOfB = A.ToB(_listOfA);


# Option 4

_listOfBs.AddMapped(a);

Her bruger vi en extension metode til at gennemskue begge typerne. Dette virker dog kun i tilfælge hvor vi har en reference. For simple assignments kunne det se sådan her ud.

a.MapTo(ref _b);

Object initializers og metodeargumenter tror jeg ikke at denne metode kan understøtte.

# Option 5

Noget med implicitte casting operators. Det vil kræve at disse kan implementeres i en partiel klasse (og at alle de relevante klasser er partielle). Hvis det kunne laves som en extension metode ville det være lettere.

B b = a;

I et tilfælde som dette kan vi, hvis A er partiel og vi kan mappe værdierne, generere en implicit casting til B. Dette har ulempen at vi ikke kan extende mapningen med manglende felter.

# Option 6

Vi bruger en intermediary klasse:

var mapper = a.Map();

Denne klasse kan så have alverdens implicitte casting operators. Den kan også extendes med ekstra felter:

var mapper = a.Map(new { extraField });
B b = mapper;

Eller bare

B b = a.Map();

Vi kan også understøtte implicitte typer

var c = a.Map(extension: b).ToType().WithInterface<ISyncCommand>();

Her vil ToType skifte typen om til at returnere en ny klasse som er unionen mellem A og B. Yderligere metoder eksisterer på denne klasse til fx at give den et interface. Jeg ved ikke om dette er brugbart i virkeligheden. Det er jo effektivt bare en slags anonym klasse man laver, så den ville være svær at consume.

Mapperen kan have forskellige metoder/overloads til at ændre mapningen:

var b = a.Map().Extend(new { myValue }); // Adds new fields and possibly replaces existing values
var b = a.Map().Replace(v => v.MyNumber, 1); // Replaces a value from "a"
var b = a.Map().Rename(v => v.MyNumber, nameof(B.MyValue)); // When the names do not match
var b = a.Map().Change(v => v.MyNumber, n => n.ToString()); // Changes the type of the property mapping

Each of these can be chained together and they feed into the conversion operator. What happens if you use the same mapper multiple times? Their uses could contaminate each other unless we generate an entirely new mapper class each place it is used. The problem is that this is not possible - the creation of the class is decoupled from the mapping, so the mappings just appear as needed. The same mapper instance could be reused several times if desired. Of course, the mutator functions could each return a new mapper class. We can reuse mapper classes as long as they have all the same mutations.

How do we get the Map method in the first place? Should each type just get an extension method? That seems a bit excessive. We could annotate the class with IMappable or [Mappable]. We could also use the Map.From(a) to create the mapper. Otherwise it would just have to be a convention that if you write Map on a type, this method springs into existence. After the initial Map method has been generated we can have intellisense on the mutator methods.