using System;
using System.Collections.Generic;
using System.Threading;

//-------------------------------------"    Initializing the Main System   "-------------------------------------

public class ElevatorController
{
    public int numberFloor;
    public int elevatorPerColumn;
    public int numberColumn;
    public string userDirection;
    public Battery battery;

    public ElevatorController(int numberFloor, int numberColumn, int elevatorPerColumn, string userDirection)
    {
        this.numberFloor = numberFloor;
        this.numberColumn = numberColumn;
        this.elevatorPerColumn = elevatorPerColumn;
        this.userDirection = userDirection;
        this.battery = new Battery(this.numberColumn);


    }

//-------------------------------------"    User Requests Elevator to Return to Lobby   "-------------------------------------

    public Elevator RequestElevatorReturning(int FloorNumber, int RequestedFloor)
    {
        Thread.Sleep(200);
        Console.WriteLine("Call Button Activated");
        Console.WriteLine("====================================================");
        Console.WriteLine("Request elevator to floor : " + FloorNumber);
        Console.WriteLine("====================================================");
        Thread.Sleep(200);
        var column = battery.SelectAppropriateColumn(FloorNumber);
        userDirection = "down";
        var elevator = column.selectOptimalElevatorReturning(FloorNumber, userDirection);
        if (elevator.elevatorFloor > FloorNumber)
        {
            elevator.addtoFloorQueue(FloorNumber, column.columnNumber);
            Console.WriteLine("Lobby Button Activated");
            Console.WriteLine("====================================================");
            elevator.addtoFloorQueue(RequestedFloor, column.columnNumber);
        }

        else if (elevator.elevatorFloor < FloorNumber)
        {
           
            elevator.addtoFloorQueue(FloorNumber, column.columnNumber);
            Console.WriteLine("Lobby Button Activated");
            Console.WriteLine("====================================================");
            elevator.addtoFloorQueue(RequestedFloor, column.columnNumber);
        }

        return elevator;
        
    }

//-------------------------------------"    User Requests Elevator From Lobby   "-------------------------------------
    public Elevator RequestElevator(int RequestedFloor)
    {
        Thread.Sleep(200);
        Console.WriteLine("Requested floor : " + RequestedFloor);
        Thread.Sleep(200);
        Console.WriteLine("Call Button Activated");


        Column column = battery.SelectAppropriateColumn(RequestedFloor);
        userDirection = "up";
        var FloorNumber = 1;
        var elevator = column.selectOptimalElevator(RequestedFloor, FloorNumber, userDirection);

        elevator.addtoFloorQueue(FloorNumber, column.columnNumber);
        elevator.addtoFloorQueue(RequestedFloor, column.columnNumber);



        return elevator;
    }


//-------------------------------------"    Initializing Battery    "-------------------------------------

    public class Battery
    {
        public string batteryStatus;
        public int numberColumn;
        public List<Column> columnList;


        public Battery(int numberColumn)
        {
            this.numberColumn = numberColumn;
            this.batteryStatus = "Activated";
            columnList = new List<Column>();



            char cols = 'A';
            for (int i = 0; i < this.numberColumn; i++, cols++)
            {

                Column column = new Column(cols, 66, 5);

                column.columnNumber = cols;
                columnList.Add(column);

            }
        }

//-------------------------------------"    Selecting the Appropriate Column    "-------------------------------------

        public Column SelectAppropriateColumn(int RequestedFloor)
        {
            Column appropriateColumn = null;
            foreach (Column column in columnList)
            {
                if (RequestedFloor > -6 && RequestedFloor <= -1 || RequestedFloor == 1)
                {
                    appropriateColumn = columnList[0];
                }
                else if (RequestedFloor >= 1 && RequestedFloor <= 20 || RequestedFloor == 1)
                {

                    appropriateColumn = columnList[1];


                }
                else if (RequestedFloor >= 21 && RequestedFloor <= 40 || RequestedFloor == 1)
                {
                    appropriateColumn = columnList[2];


                }
                else if (RequestedFloor >= 41 && RequestedFloor <= 60 || RequestedFloor == 1)
                {
                    appropriateColumn = columnList[3];


                }

            }
            return appropriateColumn;
        }
    }

//-------------------------------------"    Initializing Column    "-------------------------------------

    public class Column
    {
        public char columnNumber;
        public int numberFloor;
        public int elevatorPerColumn;
        public List<Elevator> elevatorList;
        public List<int> lobbyCallButtonList;


        public Column(char columnNumber, int numberFloor, int elevatorPerColumn)
        {
            this.columnNumber = columnNumber;
            this.numberFloor = numberFloor;
            this.elevatorPerColumn = elevatorPerColumn;
            elevatorList = new List<Elevator>();
            lobbyCallButtonList = new List<int>();
            for (int i = 0; i < this.elevatorPerColumn; i++)
            {
                Elevator elevator = new Elevator(i, "idle", 1, "up");
                elevatorList.Add(elevator);
            }
        }

//-------------------------------------"    Selecting the Optimal Elevator  "-------------------------------------

        public Elevator selectOptimalElevatorReturning(int RequestedFloor, string userDirection)
        {
            var smallestDistance = 999;
            var optimalElevator = 0;

            for (var i = 0; i < this.elevatorList.Count; i++)
            {
                var distance = elevatorList[i].elevatorFloor - RequestedFloor;

                if (distance > 0 && distance < smallestDistance)
                {
                    smallestDistance = distance;
                    optimalElevator = i;
                }
            }
            return elevatorList[optimalElevator];
        }

        public Elevator selectOptimalElevator(int RequestedFloor, int FloorNumber, string userDirection)
        {

            foreach (var elevator in elevatorList)
                if (elevator.status == "idle")
                {
                    return elevator;
                }
                else if (elevator.status == "stopped" || FloorNumber == RequestedFloor)
                {
                    return elevator;
                }
            var optimalElevator = 0;
            var smallestDistance = 1000;
            for (var i = 0; i < this.elevatorList.Count; i++)
            {
                var distance = Math.Abs(elevatorList[i].elevatorFloor - elevatorList[i].floorQueue[0]) + Math.Abs(elevatorList[i].floorQueue[0] - 1);
                if (smallestDistance >= distance)
                {
                    smallestDistance = distance;
                    optimalElevator = i;
                }
            }
            return elevatorList[optimalElevator];
        }
    }
    
//-------------------------------------"    Initializing Elevator  "-------------------------------------

    public class Elevator
    {
        public int elevatorNumber;
        public string status;
        public int elevatorFloor;
        public string elevatorDirection;
        public bool doorClearance;
        public int FloorDisplay;
        public List<int> floorQueue;

        public Elevator(int elevatorNumber, string status, int elevatorFloor, string elevatorDirection)
        {
            this.elevatorNumber = elevatorNumber;
            this.status = status;
            this.elevatorFloor = elevatorFloor;
            this.elevatorDirection = elevatorDirection;
            this.FloorDisplay = elevatorFloor;
            this.doorClearance = true;
            this.floorQueue = new List<int>();
        }

//-------------------------------------"    Pushing and Sorting the Requested Elevator in the Floor Queue  "-------------------------------------

        public void addtoFloorQueue(int RequestedFloor, char columnNumber)
        {
            floorQueue.Add(RequestedFloor);
            if (RequestedFloor > elevatorFloor)
            {
                floorQueue.Sort((a, b) => a.CompareTo(b));
            }
            else if (RequestedFloor < elevatorFloor)
            {
                floorQueue.Sort((a, b) => -1 * a.CompareTo(b));

            }

            MoveElevator(RequestedFloor, columnNumber);
        }

//-------------------------------------"    Moving the Elevator  "-------------------------------------

        public void MoveElevator(int RequestedFloor, char columnNumber)
        {
            if (RequestedFloor == elevatorFloor)
            {
                OpenDoors();
                this.status = "moving";

                this.floorQueue.Remove(0);
            }
            else if (RequestedFloor < this.elevatorFloor)
            {
                status = "moving";
                Console.WriteLine("Column: " + columnNumber + " Elevator: " + this.elevatorNumber + " is " + status);
                Console.WriteLine("====================================================");
                this.elevatorDirection = "down";
                MoveDown(RequestedFloor, columnNumber);
                this.status = "stopped";
                Console.WriteLine("Column: " + columnNumber + " Elevator: " + this.elevatorNumber + " has " + status);
                Console.WriteLine("====================================================");
                Console.WriteLine("Button Disactivated");
                
                this.OpenDoors();
                this.floorQueue.Remove(0);
            }
            else if (RequestedFloor > this.elevatorFloor)
            {
                Thread.Sleep(700);
                this.status = "moving";
                Console.WriteLine("Column: " + columnNumber + " Elevator: " + this.elevatorNumber + " is " + status);
                Console.WriteLine("====================================================");
                this.elevatorDirection = "up";
                this.MoveUp(RequestedFloor, columnNumber);
                this.status = "stopped";
                Console.WriteLine("====================================================");
                Console.WriteLine("Column: " + columnNumber + " Elevator: " + this.elevatorNumber + " has " + status);
                Console.WriteLine("====================================================");
                Console.WriteLine("Button Disactivated");

                this.OpenDoors();

                this.floorQueue.Remove(0);
            }

        }

//-------------------------------------"    Moving Up  "-------------------------------------

        public void MoveUp(int RequestedFloor, char columnNumber)
        {
            Console.WriteLine("Column: " + columnNumber + " Elevator: " + elevatorNumber + "  Current Floor: " + this.elevatorFloor);
            Thread.Sleep(700);
            Console.WriteLine("====================================================");
            while (this.elevatorFloor != RequestedFloor)
            {
                this.elevatorFloor += 1;
                Console.WriteLine("Column: " + columnNumber + " Elevator: " + elevatorNumber + "  Floor: " + this.elevatorFloor);

                Thread.Sleep(250);
            }
          
        }

//-------------------------------------"    Moving Down  "-------------------------------------

        public void MoveDown(int RequestedFloor, char columnNumber)
        {
            Console.WriteLine("Column: " + columnNumber + " Elevator: " + elevatorNumber + "  Current Floor: " + this.elevatorFloor);
            Thread.Sleep(700);
            Console.WriteLine("====================================================");

            while (this.elevatorFloor != RequestedFloor)
            {
                this.elevatorFloor -= 1;
                Console.WriteLine("Column: " + columnNumber + " Elevator: " + elevatorNumber + "  Floor: " + this.elevatorFloor);

                Thread.Sleep(250);
            }
            Console.WriteLine("====================================================");

        }

//-------------------------------------"    Doors Status    "-------------------------------------

        public void OpenDoors()
        {
            Thread.Sleep(700);

            Console.WriteLine("====================================================");
            Console.WriteLine("Doors Opening");

            Thread.Sleep(700);
            Console.WriteLine("Doors are Opened");
            Thread.Sleep(700);

            this.CloseDoors();
        }
        public void CloseDoors()
        {
            if (doorClearance == true)
            {
                Console.WriteLine("Doors Closing");
                Thread.Sleep(700);
                Console.WriteLine("Doors are Closed");
                Thread.Sleep(700);


                Console.WriteLine("====================================================");
            }
            else if (doorClearance == false)
            {
                OpenDoors();
            }
        }
    }
}

//-------------------------------------"   Scenario Testing   "-------------------------------------

    public class CommercialControllerTesting
    {
        public static void Main(string[] args)
        {
                
//-------------------------------------"   Scenario 1   "-------------------------------------  

        void Scenario1() 
        {
            ElevatorController controller = new ElevatorController(66, 4, 5, "up");

            controller.battery.columnList[1].elevatorList[0].elevatorFloor = 20;
            controller.battery.columnList[1].elevatorList[0].elevatorDirection = "down";
            controller.battery.columnList[1].elevatorList[0].status = "moving";
            controller.battery.columnList[1].elevatorList[0].floorQueue.Add(5);


            controller.battery.columnList[1].elevatorList[1].elevatorFloor = 3;
            controller.battery.columnList[1].elevatorList[1].elevatorDirection = "up";
            controller.battery.columnList[1].elevatorList[1].status = "moving";
            controller.battery.columnList[1].elevatorList[1].floorQueue.Add(15);


            controller.battery.columnList[1].elevatorList[2].elevatorFloor = 13;
            controller.battery.columnList[1].elevatorList[2].elevatorDirection = "down";
            controller.battery.columnList[1].elevatorList[2].status = "moving";
            controller.battery.columnList[1].elevatorList[2].floorQueue.Add(1);


            controller.battery.columnList[1].elevatorList[3].elevatorFloor = 15;
            controller.battery.columnList[1].elevatorList[3].elevatorDirection = "down";
            controller.battery.columnList[1].elevatorList[3].status = "moving";
            controller.battery.columnList[1].elevatorList[3].floorQueue.Add(2);
           


            controller.battery.columnList[1].elevatorList[4].elevatorFloor = 6;
            controller.battery.columnList[1].elevatorList[4].elevatorDirection = "down";
            controller.battery.columnList[1].elevatorList[4].status = "moving";
            controller.battery.columnList[1].elevatorList[4].floorQueue.Add(1);



            controller.RequestElevator(20);
        }

//-------------------------------------"   Scenario 2   "-------------------------------------

        void Scenario2() 
        {
            ElevatorController controller = new ElevatorController(66, 4, 5, "up");

            controller.battery.columnList[2].elevatorList[0].elevatorFloor = 1;
            controller.battery.columnList[2].elevatorList[0].elevatorDirection = "up";
            controller.battery.columnList[2].elevatorList[0].status = "stopped";
            controller.battery.columnList[2].elevatorList[0].floorQueue.Add(21);


            controller.battery.columnList[2].elevatorList[1].elevatorFloor = 23;
            controller.battery.columnList[2].elevatorList[1].elevatorDirection = "up";
            controller.battery.columnList[2].elevatorList[1].status = "moving";
            controller.battery.columnList[2].elevatorList[1].floorQueue.Add(28);


            controller.battery.columnList[2].elevatorList[2].elevatorFloor = 33;
            controller.battery.columnList[2].elevatorList[2].elevatorDirection = "down";
            controller.battery.columnList[2].elevatorList[2].status = "moving";
            controller.battery.columnList[2].elevatorList[2].floorQueue.Add(1);


            controller.battery.columnList[2].elevatorList[3].elevatorFloor = 40;
            controller.battery.columnList[2].elevatorList[3].elevatorDirection = "down";
            controller.battery.columnList[2].elevatorList[3].status = "moving";
            controller.battery.columnList[2].elevatorList[3].floorQueue.Add(24);


            controller.battery.columnList[2].elevatorList[4].elevatorFloor = 39;
            controller.battery.columnList[2].elevatorList[4].elevatorDirection = "down";
            controller.battery.columnList[2].elevatorList[4].status = "moving";
            controller.battery.columnList[2].elevatorList[4].floorQueue.Add(1);

            controller.RequestElevator(36);
        }

//-------------------------------------"   Scenario 3   "------------------------------------- 

        void Scenario3() 
        {
            ElevatorController controller = new ElevatorController(66, 4, 5, "down");   

            controller.battery.columnList[3].elevatorList[0].elevatorFloor = 58;
            controller.battery.columnList[3].elevatorList[0].elevatorDirection = "down";
            controller.battery.columnList[3].elevatorList[0].status = "moving";
            controller.battery.columnList[3].elevatorList[0].floorQueue.Add(1);


            controller.battery.columnList[3].elevatorList[1].elevatorFloor = 50;
            controller.battery.columnList[3].elevatorList[1].elevatorDirection = "up";
            controller.battery.columnList[3].elevatorList[1].status = "moving";
            controller.battery.columnList[3].elevatorList[1].floorQueue.Add(60);


            controller.battery.columnList[3].elevatorList[2].elevatorFloor = 46;
            controller.battery.columnList[3].elevatorList[2].elevatorDirection = "up";
            controller.battery.columnList[3].elevatorList[2].status = "moving";
            controller.battery.columnList[3].elevatorList[2].floorQueue.Add(58);


            controller.battery.columnList[3].elevatorList[3].elevatorFloor = 1;
            controller.battery.columnList[3].elevatorList[3].elevatorDirection = "up";
            controller.battery.columnList[3].elevatorList[3].status = "moving";
            controller.battery.columnList[3].elevatorList[3].floorQueue.Add(54);


            controller.battery.columnList[3].elevatorList[4].elevatorFloor = 60;
            controller.battery.columnList[3].elevatorList[4].elevatorDirection = "down";
            controller.battery.columnList[3].elevatorList[4].status = "moving";
            controller.battery.columnList[3].elevatorList[4].floorQueue.Add(1);



            controller.RequestElevatorReturning(54, 1);
        }
    
        //Scenario1();
        //Scenario2();
        Scenario3();
        }
    }

