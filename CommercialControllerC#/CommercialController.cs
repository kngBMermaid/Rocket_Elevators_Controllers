using System;

public class Battery 
{
	public int numberColumn;
    public int numberElevator;
    public int numberFloor;
	public List<Column> columnList;
	public List<int> startingFloorList;
	public List<int> endingFloorList;
	public Battery(int numberColumn, int numberElevator, int numberFloor,List<int> startingFloorList, List<int> endingFloorList)
	{
		this.numberColumn = numberColumn;
        this.numberElevator = numberElevator;
        this.numberFloor = numberFloor;
		this.columnList = new List<Column>();
		this.startingFloorList = startingFloorList;
		this.endingFloorList = endingFloorList;
		for (int i = 0; i < numberColumn; i++)
		{
			Column column = new Column(i, numberFloor, numberElevator, startingFloorList, endingFloorList);
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

    public Column(int id, int numberOfFloorToServe, int numberOfElevator, List<int> startingFloorList, List<int> endingFloorList)
    {
        this.id = id + 1;
        this.numberFloor = numberFloor;
        this.numberElevator = numberElevator;
        this.outsideButtonList = new List<OutsideButton>();
        this.elevatorList = new List<Elevator>();
        this.startingFloor = startingFloorList[id];
        this.endingFloor = endingFloorList[id];
        for (int i = 0; i < this.numberElevator; i++) {
            Elevator elevator = new Elevator(i, 1, numberFloor, startingFloor, endingFloor);
            this.elevatorList.Add(elevator);
        }
        this.InitOutsideButtonList();
    }
}

public class Elevator
{
	public int id;
	public string direction;
	public int numberFloor;
	public int currentFloor;
	public string status;
	public List<int> queue;
	public List<InsideButton> insideButtonList; 
	public string door = "CLOSED";
	public bool sameDirection = false;
	public int myCase;
	public int startingFloor;
    public int endingFloor;
	public Elevator(int id, int currentFloor, int numberFloor, int startingFloor, int endingFloor)
	{
		this.id = id + 1;												
		this.direction = "";											
		this.numberFloor = numberFloor; 
		this.currentFloor = currentFloor;								
		this.status = "IDLE";											
		this.queue = new List<int>();
		this.insideButtonList = new List<InsideButton>();
		this.door = "CLOSED";												
		this.myCase = 0;
		this.sameDirection = false;											
		this.startingFloor = startingFloor;									
        this.endingFloor = endingFloor;                                     
        this.insideButtonList.Add(buttonFirstFloor);	
   		for (int i = startingFloor; i <= endingFloor; i++)
		{
				InsideButton button = new InsideButton(i, false);
                this.insideButtonList.Add(button);			
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