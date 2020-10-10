package main

import (
	"fmt"
	"sort"
	"time"
)

//-------------------------------------"    Main System Structure   "-------------------------------------

type ElevatorController struct {
	numberBattery int
	battery       []Battery
	numberColumn   int
	userDirection  string
}

//-------------------------------------"    Battery Structure   "-------------------------------------

type Battery struct {
	numberColumn int
	columnList   []Column
}

//-------------------------------------"    Column Structure   "-------------------------------------

type Column struct {
	columnNumber               int
	elevatorPerColumn int
	elevatorList           []Elevator
}

//-------------------------------------"    Elevator Structure   "-------------------------------------

type Elevator struct {
	elevatorNumber        int
	elevatorFloor  int
	floorQueue        []int
	elevatorStatus    string
	elevatorDirection string
	doorClearance       bool
	column             int
	columnNumber               int
}

//-------------------------------------"    Creating the Battery for the Main System  "-------------------------------------

func NewController(numberBattery int) *ElevatorController {
	controller := new(ElevatorController)
	controller.numberBattery = 1
	for i := 0; i < numberBattery; i++ {
		battery := NewBattery(i)
		controller.battery = append(controller.battery, *battery)
	}
	return controller
}

//-------------------------------------"    Creating the Column in the Battery  "-------------------------------------

func NewBattery(numberColumn int) *Battery {
	battery := new(Battery)
	battery.numberColumn = 4
	for i := 0; i < battery.numberColumn; i++ {
		column := NewColumn(i)
		battery.columnList = append(battery.columnList, *column)
	}
	return battery
}

//-------------------------------------"    Creating the Elevator in the Column  "-------------------------------------

func NewColumn(elevatorPerColumn int) *Column {
	column := new(Column)
	column.elevatorPerColumn = 5
	for i := 0; i < column.elevatorPerColumn; i++ {
		elevator := NewElevator()
		column.elevatorList = append(column.elevatorList, *elevator)
	}
	return column
}

//-------------------------------------"    Creating the Elevator   "-------------------------------------

func NewElevator() *Elevator {
	elevator := new(Elevator)
	elevator.elevatorFloor = 1
	elevator.floorQueue = []int{}
	elevator.elevatorStatus = "idle"
	elevator.elevatorDirection = "up"
	elevator.doorClearance= true
	return elevator
}

//-------------------------------------"    User Requests Elevator to Return to Lobby   "-------------------------------------

func (controller *ElevatorController) RequestElevatorReturning(FloorNumber, RequestedFloor int) Elevator {
	fmt.Println("Elevator Requested on Floor : ", FloorNumber)
	var column = controller.battery[0].SelectAppropriateColumn(FloorNumber)
	controller.userDirection = "down"
	var elevator = column.SelectOptimalElevator(RequestedFloor, controller.userDirection)
	elevator.addtoFloorQueue(FloorNumber)
	elevator.addtoFloorQueue(RequestedFloor)
	return elevator
}

//-------------------------------------"    User Requests Elevator from Lobby   "-------------------------------------

func (controller *ElevatorController) RequestElevator(RequestedFloor int) Elevator {
	fmt.Println("Request elevator to floor : ", RequestedFloor)
	column := controller.battery[0].SelectAppropriateColumn(RequestedFloor)
	controller.userDirection = "up"
	var elevator = column.SelectOptimalElevator(RequestedFloor, controller.userDirection)
	var FloorNumber = 1
	elevator.addtoFloorQueue(FloorNumber)
	elevator.addtoFloorQueue(RequestedFloor)
	return elevator
}

//-------------------------------------"    Selecting the Appropriate Column  "-------------------------------------

func (b *Battery) SelectAppropriateColumn(RequestedFloor int) Column { // not sure about *
	if RequestedFloor > -6 && RequestedFloor <= 1 {
		return b.columnList[0]
	} else if RequestedFloor > 1 && RequestedFloor <= 20 {
		return b.columnList[1]
	} else if RequestedFloor > 21 && RequestedFloor <= 40 {
		return b.columnList[2]
	} else if RequestedFloor > 41 && RequestedFloor <= 60 {
		return b.columnList[3]
	}
	return b.columnList[3]
}

//-------------------------------------"    Selecting the Optimal Elevator  "-------------------------------------

func (c *Column) SelectOptimalElevator(RequestedFloor int, userDirection string) Elevator {
	var optimalElevator = c.elevatorList[0]
	for _, e := range c.elevatorList {
		if RequestedFloor < e.elevatorFloor && e.elevatorDirection == "down" && userDirection == "down" {
			optimalElevator = e
		} else if e.elevatorStatus == "idle" {
			optimalElevator = e
		} else if e.elevatorDirection != userDirection && e.elevatorStatus == "moving" || e.elevatorStatus == "stopped" {
			optimalElevator = e
		} else if e.elevatorDirection == userDirection && e.elevatorStatus == "moving" || e.elevatorStatus == "stopped" {
			optimalElevator = e
		}
	}
	return optimalElevator
}

//-------------------------------------"    Pushing and Sorting the Requested Elevator in the Floor Queue  "-------------------------------------

func (e *Elevator) addtoFloorQueue(RequestedFloor int) {
	e.floorQueue = append(e.floorQueue, RequestedFloor)
	if RequestedFloor > e.elevatorFloor {

		sort.Ints(e.floorQueue)
	} else if RequestedFloor < e.elevatorFloor {

		sort.Sort(sort.Reverse(sort.IntSlice(e.floorQueue)))
	}
	e.MoveElevator(RequestedFloor)
}

//-------------------------------------"    Moving the Elevator   "-------------------------------------

func (e *Elevator) MoveElevator(RequestedFloor int) {
	if RequestedFloor == e.elevatorFloor {
		e.OpenDoors()
	} else if RequestedFloor > e.elevatorFloor {
		e.elevatorStatus = "moving"
		e.moveUp(RequestedFloor)
		e.elevatorStatus = "stopped"
		e.OpenDoors()
		e.elevatorStatus = "moving"
	} else if RequestedFloor < e.elevatorFloor {
		e.elevatorStatus = "moving"
		e.moveDown(RequestedFloor)
		e.elevatorStatus = "stopped"
		e.OpenDoors()
		e.elevatorStatus = "moving"
	}
}

//-------------------------------------"    Moving Up  "-------------------------------------

func (e *Elevator) moveUp(RequestedFloor int) {
	fmt.Println("Column : ", e.columnNumber, " Elevator : #", e.elevatorNumber, " Current Floor :", e.elevatorFloor)
	for RequestedFloor > e.elevatorFloor {
		e.elevatorFloor += 1
		if RequestedFloor == e.elevatorFloor {
			time.Sleep(1 * time.Second)
			fmt.Println("======================================")
			fmt.Println("Column : ", e.columnNumber, " Elevator : #", e.elevatorNumber, " Arrived at Requested Floor : ", e.elevatorFloor)
		}
		fmt.Println("Column : ", e.columnNumber, " Elevator : #", e.elevatorNumber, " Floor : ", e.elevatorFloor)
	}
}

//-------------------------------------"    Moving Down   "-------------------------------------

func (e *Elevator) moveDown(RequestedFloor int) {
	fmt.Println("Column : ", e.columnNumber, " Elevator : #", e.elevatorNumber, " Current Floor :", e.elevatorFloor)
	for RequestedFloor < e.elevatorFloor {
		e.elevatorFloor -= 1
		if RequestedFloor == e.elevatorFloor {
			time.Sleep(1 * time.Second)
			fmt.Println("======================================")
			fmt.Println("Column : ", e.columnNumber, " Elevator : #", e.elevatorNumber, " Arrived at Requested Floor : ", e.elevatorFloor)
		}
		fmt.Println("Column : ", e.columnNumber, " Elevator : #", e.elevatorNumber, " Floor : ", e.elevatorFloor)
	}
}

//-------------------------------------"    Doors Status   "-------------------------------------

func (e *Elevator) OpenDoors() {
	fmt.Println("======================================")
	fmt.Println("Doors Opening")
	fmt.Println("Doors are Open")
	e.CloseDoors()
}
func (e *Elevator) CloseDoors() {
	if e.doorClearance== true {
		fmt.Println("Doors Closing")
		fmt.Println("Doors are Close")
		fmt.Println("======================================")
	} else if e.doorClearance{
		e.OpenDoors()
	}
}

//-------------------------------------"    Scenario 1   "-------------------------------------

func (controller *ElevatorController) TestScenario1() {


	controller.battery[0].columnList[1].elevatorList[0].elevatorFloor = 20
	controller.battery[0].columnList[1].elevatorList[0].elevatorDirection = "down"
	controller.battery[0].columnList[1].elevatorList[0].elevatorStatus = "moving"
	controller.battery[0].columnList[1].elevatorList[0].floorQueue = append(controller.battery[0].columnList[1].elevatorList[0].floorQueue, 5)

	controller.battery[0].columnList[1].elevatorList[1].elevatorFloor = 3
	controller.battery[0].columnList[1].elevatorList[1].elevatorDirection = "up"
	controller.battery[0].columnList[1].elevatorList[1].elevatorStatus = "moving"
	controller.battery[0].columnList[1].elevatorList[1].floorQueue = append(controller.battery[0].columnList[1].elevatorList[1].floorQueue, 15)

	controller.battery[0].columnList[1].elevatorList[2].elevatorFloor = 13
	controller.battery[0].columnList[1].elevatorList[2].elevatorDirection = "down"
	controller.battery[0].columnList[1].elevatorList[2].elevatorStatus = "moving"
	controller.battery[0].columnList[1].elevatorList[2].floorQueue = append(controller.battery[0].columnList[1].elevatorList[2].floorQueue, 1)

	controller.battery[0].columnList[1].elevatorList[3].elevatorFloor = 15
	controller.battery[0].columnList[1].elevatorList[3].elevatorDirection = "down"
	controller.battery[0].columnList[1].elevatorList[3].elevatorStatus = "moving"
	controller.battery[0].columnList[1].elevatorList[3].floorQueue = append(controller.battery[0].columnList[1].elevatorList[3].floorQueue, 2)

	controller.battery[0].columnList[1].elevatorList[4].elevatorFloor = 6
	controller.battery[0].columnList[1].elevatorList[4].elevatorDirection = "down"
	controller.battery[0].columnList[1].elevatorList[4].elevatorStatus = "moving"
	controller.battery[0].columnList[1].elevatorList[4].floorQueue = append(controller.battery[0].columnList[1].elevatorList[4].floorQueue, 1)
	
	controller.RequestElevator(20)
}

//-------------------------------------"    Scenario 2   "-------------------------------------

func (controller *ElevatorController) TestScenario2() {


	controller.battery[0].columnList[2].elevatorList[0].elevatorFloor = 1
	controller.battery[0].columnList[2].elevatorList[0].elevatorDirection = "up"
	controller.battery[0].columnList[2].elevatorList[0].elevatorStatus = "stopped"
	controller.battery[0].columnList[2].elevatorList[0].floorQueue = append(controller.battery[0].columnList[2].elevatorList[0].floorQueue, 21)

	controller.battery[0].columnList[2].elevatorList[1].elevatorFloor = 23
	controller.battery[0].columnList[2].elevatorList[1].elevatorDirection = "up"
	controller.battery[0].columnList[2].elevatorList[1].elevatorStatus = "moving"
	controller.battery[0].columnList[2].elevatorList[1].floorQueue = append(controller.battery[0].columnList[2].elevatorList[1].floorQueue, 28)

	controller.battery[0].columnList[2].elevatorList[2].elevatorFloor = 33
	controller.battery[0].columnList[2].elevatorList[2].elevatorDirection = "down"
	controller.battery[0].columnList[2].elevatorList[2].elevatorStatus = "moving"
	controller.battery[0].columnList[2].elevatorList[2].floorQueue = append(controller.battery[0].columnList[2].elevatorList[2].floorQueue, 1)

	controller.battery[0].columnList[2].elevatorList[3].elevatorFloor = 40
	controller.battery[0].columnList[2].elevatorList[3].elevatorDirection = "down"
	controller.battery[0].columnList[2].elevatorList[3].elevatorStatus = "moving"
	controller.battery[0].columnList[2].elevatorList[3].floorQueue = append(controller.battery[0].columnList[2].elevatorList[3].floorQueue, 24)

	controller.battery[0].columnList[2].elevatorList[4].elevatorFloor = 39
	controller.battery[0].columnList[2].elevatorList[4].elevatorDirection = "down"
	controller.battery[0].columnList[2].elevatorList[4].elevatorStatus = "moving"
	controller.battery[0].columnList[2].elevatorList[4].floorQueue = append(controller.battery[0].columnList[2].elevatorList[4].floorQueue, 1)
	
	controller.RequestElevator(36)
}

//-------------------------------------"    Scenario 3   "-------------------------------------

func (controller *ElevatorController) TestScenario3() {


	controller.battery[0].columnList[3].elevatorList[0].elevatorFloor = 58
	controller.battery[0].columnList[3].elevatorList[0].elevatorDirection = "down"
	controller.battery[0].columnList[3].elevatorList[0].elevatorStatus = "moving"
	controller.battery[0].columnList[3].elevatorList[0].floorQueue = append(controller.battery[0].columnList[3].elevatorList[0].floorQueue, 1)

	controller.battery[0].columnList[3].elevatorList[1].elevatorFloor = 50
	controller.battery[0].columnList[3].elevatorList[1].elevatorDirection = "up"
	controller.battery[0].columnList[3].elevatorList[1].elevatorStatus = "moving"
	controller.battery[0].columnList[3].elevatorList[1].floorQueue = append(controller.battery[0].columnList[3].elevatorList[1].floorQueue, 60)

	controller.battery[0].columnList[3].elevatorList[2].elevatorFloor = 46
	controller.battery[0].columnList[3].elevatorList[2].elevatorDirection = "up"
	controller.battery[0].columnList[3].elevatorList[2].elevatorStatus = "moving"
	controller.battery[0].columnList[3].elevatorList[2].floorQueue = append(controller.battery[0].columnList[3].elevatorList[2].floorQueue, 58)

	controller.battery[0].columnList[3].elevatorList[3].elevatorFloor = 1
	controller.battery[0].columnList[3].elevatorList[3].elevatorDirection = "up"
	controller.battery[0].columnList[3].elevatorList[3].elevatorStatus = "moving"
	controller.battery[0].columnList[3].elevatorList[3].floorQueue = append(controller.battery[0].columnList[3].elevatorList[3].floorQueue, 54)

	controller.battery[0].columnList[3].elevatorList[4].elevatorFloor = 60
	controller.battery[0].columnList[3].elevatorList[4].elevatorDirection = "down"
	controller.battery[0].columnList[3].elevatorList[4].elevatorStatus = "moving"
	controller.battery[0].columnList[3].elevatorList[4].floorQueue = append(controller.battery[0].columnList[3].elevatorList[4].floorQueue, 1)
	
	controller.RequestElevator(36)
}

//-------------------------------------"    Scenario 4   "-------------------------------------

func (controller *ElevatorController) TestScenario4() {


	controller.battery[0].columnList[0].elevatorList[0].elevatorFloor = -4
	controller.battery[0].columnList[0].elevatorList[0].elevatorStatus = "idle"

	controller.battery[0].columnList[0].elevatorList[1].elevatorFloor = 1
	controller.battery[0].columnList[0].elevatorList[1].elevatorStatus = "idle"

	controller.battery[0].columnList[0].elevatorList[2].elevatorFloor = -3
	controller.battery[0].columnList[0].elevatorList[2].elevatorDirection = "down"
	controller.battery[0].columnList[0].elevatorList[2].elevatorStatus = "moving"
	controller.battery[0].columnList[0].elevatorList[2].floorQueue = append(controller.battery[0].columnList[0].elevatorList[2].floorQueue, -5)

	controller.battery[0].columnList[0].elevatorList[3].elevatorFloor = -6
	controller.battery[0].columnList[0].elevatorList[3].elevatorDirection = "up"
	controller.battery[0].columnList[0].elevatorList[3].elevatorStatus = "moving"
	controller.battery[0].columnList[0].elevatorList[3].floorQueue = append(controller.battery[0].columnList[0].elevatorList[3].floorQueue, 1)

	controller.battery[0].columnList[0].elevatorList[4].elevatorFloor = -1
	controller.battery[0].columnList[0].elevatorList[4].elevatorDirection = "down"
	controller.battery[0].columnList[0].elevatorList[4].elevatorStatus = "moving"
	controller.battery[0].columnList[0].elevatorList[4].floorQueue = append(controller.battery[0].columnList[0].elevatorList[4].floorQueue, -6)
	
	controller.RequestElevator(36)
}


func main() {
	controller := NewController(1)
	controller.TestScenario1()
	//controller.TestScenario2()
	//controller.TestScenario3()
	//controller.TestScenario4()
}