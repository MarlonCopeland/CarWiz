using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CarWiz
{
    class Program
    {
        static void Main(string[] args)
        {
            //            /****************Car Comparison********************/

            //            Data:
            //            Make Model       Color Year    Price TCC Rating Hwy MPG
            //Honda       CRV Green   2016    $23,845     8               33
            //Ford Escape      Red     2017    $23,590     7.8             32
            //Hyundai Sante Fe Grey    2016    $24,950     8               27
            //Mazda CX-5        Red     2017    $21,795     8               35
            //Subaru Forester    Blue    2016    $22,395     8.4             32


            //Create tool based on this sample data that will give the following:
            //            1) A function to calculate newest vehicles in order Done
            //2) A function to calculate alphabetized List of vehicles Done
            //3) A function to calculate ordered List of Vehicles by Price Done
            //4) A function to calculate the best value Done
            //5) A function to calculate full consumption for a given distance Done
            //6) A function to return a random car from the list Done
            //7) A function to return average MPG by year Done


            // inputs
            Console.WriteLine("Car Wiz (All Commands are case sensitive)");
            Console.WriteLine("Press A then enter to display Car Data Alphabetically by Make");
            Console.WriteLine("Press N then enter to display Car Data by Newest");
            Console.WriteLine("Press P then enter to display Car Data by Price");
            Console.WriteLine("Press V then enter to display Car Data sorted by the Best Value");

            Console.WriteLine("Press F then enter to display Fuel Consumption for a given Distance");
            Console.WriteLine("Press R then enter to display Car Data for a Randomly Selected Car");

            Console.WriteLine("Press G then enter to display the average MPG by year");

            Console.WriteLine("Press q then enter to quit");

            XElement CarsXml;

            try
            {
                 CarsXml = XElement.Load("../../CarData.xml");
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error Loading the Xml file Read the Error Below then press any key to exit.");
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                throw;
            }


            //input loop
            while (true)
            {
                string strCommand = Console.ReadLine();

                switch (strCommand)
                {
                    case "q":
                        Console.WriteLine("Quitting Tool");
                        Environment.Exit(0);
                        break;
                    case "A":
                        ReturnSortedSet(CarsXml, "Alpha");

                        break;
                    case "N":
                        ReturnSortedSet(CarsXml, "Newest");

                        break;
                    case "P":
                        ReturnSortedSet(CarsXml, "Price");
                        break;
                    case "V":
                        ReturnSortedSet(CarsXml, "Value");
                        break;
                    case "R":
                        DisplayRandomCar(CarsXml);

                        break;
                    case "G":
                        DisplayAvgMPGbyYear(CarsXml);
                        break;
                    case "F":
                        Console.WriteLine("Please Enter Distance");
                        if (Int32.TryParse(Console.ReadLine(), out int distance)){
                            DisplayFuelConsumption(CarsXml, distance);
                        }
                        else {
                            Console.WriteLine("Please Input a number without a decimal for this operation");
                        }
                        break;
                    default:
                        break;
                }


            }

            /// use linq to xml to quickly sort based on elements
            /// use console.writeline to output results
            void ReturnSortedSet(XElement carsRoot, string sortType)
            {
                //sort using linq to xml
                IEnumerable<XElement> Cars;

                switch (sortType)
                {
                    case "Newest":
                        Cars =
                            from el in carsRoot.Elements("Car")
                            orderby (int)el.Element("Year") descending
                            select el;
                        break;
                    case "Alpha":
                        Cars =
                            from el in carsRoot.Elements("Car")
                            orderby (string)el.Element("Make"), (string)el.Element("Model")
                            select el;
                        break;
                    case "Price":
                        Cars =
                            from el in carsRoot.Elements("Car")
                            orderby (double)el.Element("Price")
                            select el;
                        break;
                    case "Value": //TCC Rating??
                        Cars =
                            from el in carsRoot.Elements("Car")
                            orderby (double)el.Element("TCCRating") descending
                            select el;
                        break;
                    case "Fuel":
                        Cars =
                            from el in carsRoot.Elements("Car")
                            orderby (string)el.Element(sortType)
                            select el;
                        break;
                    default:
                        Cars = null;
                        break;
                }

                Console.WriteLine("Make      Model     Year     Price     TCC Rating     HWY MPG");
                Console.WriteLine();
                if (Cars != null)
                    foreach (XElement c in Cars)
                    {
                        Console.WriteLine(c.Element("Make").Value + "     " + c.Element("Model").Value +
                           "     " + c.Element("Year").Value +
                           "        " + c.Element("Price").Value +
                           "        " + c.Element("TCCRating").Value +
                           "        " + c.Element("MPG").Value);
                    }
            }

            /// using linq and simple division here
            void DisplayFuelConsumption(XElement carsRoot, double distance)
            {
                IEnumerable<XElement> Cars;

                Cars = from el in carsRoot.Elements("Car")
                       select el;

                Console.WriteLine("Make      Model     HWY MPG     Fuel Consumption");
                Console.WriteLine();
                if (Cars != null)
                    foreach (XElement c in Cars)
                    {
                        double fuelCalc = distance / Double.Parse(c.Element("MPG").Value);

                        Console.WriteLine(c.Element("Make").Value + "     " + c.Element("Model").Value +
                           "        " + c.Element("MPG").Value +
                           "      " + Math.Round(fuelCalc, 2 ) + " Gallons");

                           
                    }

            }

            //using linq to xml grouping along with Average on new object
            void DisplayAvgMPGbyYear(XElement carsRoot)
            {
                // IEnumerable<XElement> Cars;


                var carsv = from el in carsRoot.Descendants("Car")
                            group el by (string)el.Element("Year") into d
                                   select new { year = d.Key, mpg = d.Average(x => (int)x.Element("MPG"))};

                Console.WriteLine("Year     AVG MPG");
                Console.WriteLine();

                if (carsv != null)
                    foreach (var c in carsv)
                    {
                        Console.WriteLine(c.year + "    " + c.mpg);
                     
                    }



            }

            //using linq and Random to return one element
            void DisplayRandomCar(XElement carsRoot)
            {
                IEnumerable<XElement> Cars;

                Cars = from el in carsRoot.Elements("Car")
                       select el;

                Random rand = new Random();

                int randCarIndex = rand.Next(0, Cars.Count() - 1);

                Console.WriteLine(Cars.ElementAt(randCarIndex));
            }

        }


    }

}
