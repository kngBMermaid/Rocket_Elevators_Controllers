using System;

public class Battery
{
    public int numberColumns;
    public int numberElevators;
    public int numberFloors;
    public List<Column> columnList;
    public List<int> startingFloorList;
    public List<int> endingFloorList;
    public Battery(int numberColumns, int numberElevators, int numberFloors, List<int> startingFloorList, List<int> endingFloorList)
    {
        this.numberColumns = numberColumns;
        this.numberElevators = numberElevators;
        this.numberFloors = numberFloors;
        this.columnList = new List<Column>();
        this.startingFloorList = startingFloorList;
        this.endingFloorList = endingFloorList;
        for (int i = 0; i < numberColumns; i++)
        {
            Column column = new Column(i, numberFloors, numberElevators, startingFloorList, endingFloorList);
            this.columnList.Add(column);
        }
    }
}


public class Column
{
    public int id;
    public int numberFloors;
    public int numberElevators;
    public List<OutsideButton> outsideButtonList;
    public List<Elevator> elevatorList;
    public int startingFloor;
    public int endingFloor;

    public Column(int id, int numberFloors, int numberElevators, List<int> startingFloorList, List<int> endingFloorList)
    {
        this.id = id + 1;
        this.numberFloors = numberFloors;
        this.numberElevators = numberElevators;
        this.outsideButtonList = new List<outsideButton>();
        this.elevatorList = new List<Elevator>();
        this.startingFloor = startingFloorList[id];
        this.endingFloor = endingFloorList[id];
        for (int i = 0; i < this.numberElevators; i++)
        {
            Elevator elevator = new Elevator(i, 1, numberFloors, startingFloor, endingFloor);
            this.elevatorList.Add(elevator);
        }
        this.InitOutsideButtonList();
    }
    public void InitOutsideButtonList()
    {
        OutsideButton lobbyRequestButton = new OutsideButton(1, "UP", false);
        this.outsideButtonList.Add(lobbyRequestButton);
        for (int i = startingFloor; i <= endingFloor; i++)
        {
            if (i != endingFloor)
            {
                OutsideButton buttonUp = new OutsideButton(i, "UP", false);
                this.outsideButtonList.Add(buttonUp);
            }
            if (i != 1)
            {
                OutsideButton buttonDown = new OutsideButton(i, "DOWN", false);
                this.outsideButtonList.Add(buttonDown);
            }
        }
    }
    public Elevator SelectOptimalElevator(int requestedFloor, string direction)
    {
        int shortestDistance = numberFloors;
        int OptimalElevator = 0;
        for (int i = 0; i < elevatorList.Count; i++)
        {
            if (((elevatorList[i].status == "IDLE") || (elevatorList[i].direction == "UP" && direction == "UP" && (requestedFloor >= elevatorList[i].currentFloor))) || (elevatorList[i].direction == "DOWN" && direction == "DOWN" && (requestedFloor <= elevatorList[i].currentFloor)))
            {
                elevatorList[i].myCase = 1;
            }
            if (((elevatorList[i].direction == "UP" && direction == "UP" && (requestedFloor < elevatorList[i].currentFloor))) || ((elevatorList[i].direction == "DOWN" && direction == "DOWN" && (requestedFloor > elevatorList[i].currentFloor))) || ((elevatorList[i].direction == "DOWN" && direction == "UP" && (requestedFloor >= elevatorList[i].currentFloor))) || ((elevatorList[i].direction == "DOWN" && direction == "UP" && (requestedFloor >= elevatorList[i].currentFloor))) || (elevatorList[i].direction == "DOWN" && direction == "UP" && (requestedFloor < elevatorList[i].currentFloor)))
            {
                elevatorList[i].myCase = 2;
            }
        }
        for (int i = 0; i < this.elevatorList.Count; i++)
        {
            if (elevatorList[i].myCase == 1)
            {
                int Distance = Math.Abs(this.elevatorList[i].currentFloor - requestedFloor);
                if (Distance < shortestDistance)
                {
                    OptimalElevator = this.elevatorList[i].id;
                    shortestDistance = Distance;
                }
            }
            else if (elevatorList[i].myCase == 2)
            {
                OptimalElevator = this.elevatorList[i].id;
                int Distance = Math.Abs(this.elevatorList[i].currentFloor - elevatorList[i].floorQueue[0]) + Math.Abs(elevatorList[i].floorQueue[0] - requestedFloor);
                shortestDistance = Distance;
            }
        }
        return this.SelectOptimalColumn(OptimalElevator);
    }
    public Elevator CallElevator(int requestedFloor, string direction)
    {
        Elevator elevator = SelectOptimalElevator(requestedFloor, direction);
        elevator.AddtoFloorQueue(requestedFloor);
        elevator.MoveElevator();
        return elevator;
    }
    public void CallFloor(Elevator elevator, int requestedFloor)
    {
        elevator.AddtoFloorQueue(requestedFloor);
        elevator.CloseDoors();
        elevator.MoveElevator();
    }
}



public class Elevator
{
    public int id;
    public string direction;
    public int numberFloors;
    public int currentFloor;
    public string status;
    public List<int> floorQueue;
    public List<InsideButton> insideButtonList;
    public string door = "CLOSED";
    public bool sameDirection = false;
    public int myCase;
    public int startingFloor;
    public int endingFloor;
    public Elevator(int id, int currentFloor, int numberFloors, int startingFloor, int endingFloor)
    {
        this.id = id + 1;
        this.direction = "";
        this.numberFloors = numberFloors;
        this.currentFloor = currentFloor;
        this.status = "IDLE";
        this.floorQueue = new List<int>();
        this.insideButtonList = new List<InsideButton>();
        this.door = "CLOSED";
        this.myCase = 0;
        this.sameDirection = false;
        this.startingFloor = startingFloor;
        this.endingFloor = endingFloor;
        InsideButton lobbyRequestButton = new InsideButton(1, false);
        this.insideButtonList.Add(lobbyRequestButton);
        for (int i = startingFloor; i <= endingFloor; i++)
        {
            InsideButton button = new InsideButton(i, false);
            this.insideButtonList.Add(button);
        }
    }
    public void AddtoFloorQueue(int requestedFloor)
    {
        this.floorQueue.Add(requestedFloor);
        if (this.direction == "UP")
        {
            this.floorQueue.Sort((a, b) => a - b);
        }
        if (this.direction == "DOWN")
        {
            this.floorQueue.Sort((a, b) => b - a);
        }
    }
    public void MoveElevator()
    {
        Console.WriteLine("Start Elevator Movement");
        while (this.floorQueue.Count > 0)
        {
            if (this.door == "OPENED") { this.CloseDoors(); }
            int firstElement = this.floorQueue[0];
            if (firstElement == this.currentFloor)
            {
                this.floorQueue.RemoveAt(0);
                this.OpenDoors();
                Console.WriteLine($"Door for: {firstElement}");
            }
            if (firstElement > this.currentFloor)
            {
                this.status = "MOVING";
                this.direction = "UP";
                this.MoveUp();
            }
            if (firstElement < this.currentFloor)
            {
                this.status = "MOVING";
                this.direction = "DOWN";
                this.MoveDown();
            }
        }
        if (this.floorQueue.Count > 0) { }
        else
        {
            Console.WriteLine("Elevator Status: Idle");
            this.status = "IDLE";
        }
    }
    public void MoveUp()
    {
        this.currentFloor = (this.currentFloor + 1);
        Console.WriteLine($"Current floor: {this.currentFloor}");
    }

    public void MoveDown()
    {
        this.currentFloor = (this.currentFloor - 1);
        Console.WriteLine($"Current floor: {this.currentFloor}");
    }


    public void OpenDoors()
    {
        this.door = "OPENED";
    }


    public void CloseDoors()
    {
        this.door = "CLOSED";
    }

}


public class InsideButton
{
    public int floor;
    public bool activated;

    public InsideButton(int floor, bool activated)
    {
        this.floor = floor;
        this.activated = false;
    }
}


public class OutsideButton
{
    public int floor;
    public string direction;
    public bool activated;

    public OutsideButton(int floor, string direction, bool activated)
    {
        this.floor = floor;
        this.direction = direction;
        this.activated = false;
    }
}