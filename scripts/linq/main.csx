class Apple {
    public string Color {get; set;}
    public int Weight {get; set;}
}

List<Apple> apples = new List<Apple> {
    new Apple{ Color = "Red", Weight=180},
    new Apple{ Color = "Green", Weight=195},
    new Apple{ Color = "Red", Weight=190},
    new Apple{ Color = "Green", Weight=185}
};

IEnumerable<int> weightOfRedApples = apples
                                .Where(apple => apple.Color == "Red")
                                .Select(apple => apple.Weight);

double average = weightOfRedApples.Average();

//Qual è il peso minimo?
int minWeight = apples.Select(apple => apple.Weight).Min();
Console.WriteLine(minWeight);

//Di che colore è la mela che pesa 190 grammi?
IEnumerable<string> apple190Color = apples.Where(apple => apple.Weight == 190).Select(apple => apple.Color);
foreach(var apple in apple190Color) {
    Console.WriteLine(apple);
}

//Quante sono le mele rosse?
int numberOfApples = apples.Where(apple => apple.Color == "Red").Count();
Console.WriteLine(numberOfApples);

//Quanto pesano in totale le mele verdi?
int greenAppleWeight = apples.Where(apple => apple.Color == "Green").Select(apple => apple.Weight).Sum();
Console.WriteLine(greenAppleWeight);

Console.WriteLine();