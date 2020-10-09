package main

import (
	"fmt"
	"sort"
	"time"
)

//-------------------------------------"    Main System Structure   "-------------------------------------

type ElevatorController struct {
	numberBatteries int
	batteries       []Battery
	numberColumns   int
	userDirection  string
}

//-------------------------------------"    Battery Structure   "-------------------------------------

type Battery struct {
	numberColumns int
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
	elevatorPosition  int
	floorQueue        []int
	elevatorStatus    string
	elevatorDirection string
	doorClearance       bool
	column             Column
}

//-------------------------------------"    Creating the Battery for the Main System  "-------------------------------------

func NewController(numberBatteries int) ElevatorController {
	controller := new(ElevatorController)
	controller.numberBatteries = 1
	for index := 0; index < numberBatteries; index++ {
		battery := NewBattery(index)
		controller.batteries = append(controller.batteries, *battery)
	}
	return *controller
}

//-------------------------------------"    Creating the Column in the Battery  "-------------------------------------

func NewBattery(numberColumns int) *Battery {
	battery := new(Battery)
	battery.numberColumns = 4
	for index := 0; index < battery.numberColumns; index++ {
		column := NewColumn(index)
		battery.columnList = append(battery.columnList, *column)
	}
	return battery
}

//-------------------------------------"    Creating the Elevator in the Column  "-------------------------------------

func NewColumn(elevatorPerColumn int) *Column {
	column := new(Column)
	column.elevatorPerColumn = 5
	for index := 0; index < column.elevatorPerColumn; index++ {
		elevator := NewElevator()
		column.elevatorList = append(column.elevatorList, *elevator)
	}
	return column
}

//-------------------------------------"    Creating the Elevator   "-------------------------------------

func NewElevator() *Elevator {
	elevator := new(Elevator)
	elevator.elevatorPosition = 1
	elevator.floorQueue = []int{}
	elevator.elevatorStatus = "idle"
	elevator.elevatorDirection = "up"
	elevator.doorClearance= true
	return elevator
}

//-------------------------------------"    User Requests Elevator to Return to Lobby   "-------------------------------------

func (controller *ElevatorController) RequestElevatorReturning(FloorNumber, RequestedFloor int) Elevator {
	fmt.Println("Elevator Requested on Floor : ", FloorNumber)
	time.Sleep(300 * time.Millisecond)
	fmt.Println("Button Light On")
	var column = controller.batteries[0].SelectAppropriateColumn(FloorNumber)
	controller.userDirection = "down"
	var elevator = column.SelectOptimalElevator(RequestedFloor, controller.userDirection)
	elevator.addtoFloorQueue(FloorNumber)
	elevator.addtoFloorQueue(RequestedFloor)
	return elevator
}

//-------------------------------------"    User Requests Elevator from Lobby   "-------------------------------------

func (controller *ElevatorController) RequestElevator(RequestedFloor int) Elevator {
	fmt.Println("Request elevator to floor : ", RequestedFloor)
	time.Sleep(3 * time.Millisecond)
	fmt.Println("Button Light On")
	column := controller.batteries[0].SelectAppropriateColumn(RequestedFloor)
	controller.userDirection = "up"
	var elevator = column.SelectOptimalElevator(RequestedFloor, controller.userDirection)
	var FloorNumber = 1
	elevator.addtoFloorQueue(FloorNumber)
	elevator.addtoFloorQueue(RequestedFloor)
	return elevator
}

//-------------------------------------"    Selecting the Appropriate Column  "-------------------------------------

func (b *Battery) SelectAppropriateColumn(RequestedFloor int) Column { // not sure about *
	if RequestedFloor > 0 && RequestedFloor <= 22 {
		return b.columnList[0]
	} else if RequestedFloor > 22 && RequestedFloor <= 43 {
		return b.columnList[1]
	} else if RequestedFloor > 43 && RequestedFloor <= 64 {
		return b.columnList[2]
	} else if RequestedFloor > 64 && RequestedFloor <= 85 {
		return b.columnList[3]
	}
	return b.columnList[3]
}

//-------------------------------------"    Selecting the Optimal Elevator  "-------------------------------------

func (c *Column) SelectOptimalElevator(RequestedFloor int, userDirection string) Elevator {
	var optimalElevator = c.elevatorList[0]
	for _, e := range c.elevatorList {
		if RequestedFloor < e.elevatorPosition && e.elevatorDirection == "down" && userDirection == "down" {
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
	if RequestedFloor > e.elevatorPosition {

		sort.Ints(e.floorQueue)
	} else if RequestedFloor < e.elevatorPosition {

		sort.Sort(sort.Reverse(sort.IntSlice(e.floorQueue)))
	}
	e.MoveElevator(RequestedFloor)
}

//-------------------------------------"    Moving the Elevator   "-------------------------------------

func (e *Elevator) MoveElevator(RequestedFloor int) {
	if RequestedFloor == e.elevatorPosition {
		e.OpenDoors()
	} else if RequestedFloor > e.elevatorPosition {
		e.elevatorStatus = "moving"
		e.moveUp(RequestedFloor)
		e.elevatorStatus = "stopped"
		e.OpenDoors()
		e.elevatorStatus = "moving"
	} else if RequestedFloor < e.elevatorPosition {
		e.elevatorStatus = "moving"
		e.moveDown(RequestedFloor)
		e.elevatorStatus = "stopped"
		e.OpenDoors()
		e.elevatorStatus = "moving"
	}
}

//-------------------------------------"    Moving Up  "-------------------------------------

func (e *Elevator) moveUp(RequestedFloor int) {
	fmt.Println("Column : ", e.column.columnNumber, " Elevator : #", e.elevatorNumber, " Current Floor :", e.elevatorPosition)
	for RequestedFloor > e.elevatorPosition {
		e.elevatorPosition += 1
		if RequestedFloor == e.elevatorPosition {
			time.Sleep(1 * time.Second)
			fmt.Println("======================================")
			fmt.Println("Column : ", e.column.columnNumber, " Elevator : #", e.elevatorNumber, " Arrived at Requested Floor : ", e.elevatorPosition)
		}
		time.Sleep(300 * time.Millisecond)
		fmt.Println("Column : ", e.column.columnNumber, " Elevator : #", e.elevatorNumber, " Floor : ", e.elevatorPosition)
	}
}

//-------------------------------------"    Moving Down   "-------------------------------------

func (e *Elevator) moveDown(RequestedFloor int) {
	fmt.Println("Column : ", e.column.columnNumber, " Elevator : #", e.elevatorNumber, " Current Floor :", e.elevatorPosition)
	for RequestedFloor < e.elevatorPosition {
		e.elevatorPosition -= 1
		if RequestedFloor == e.elevatorPosition {
			time.Sleep(1 * time.Second)
			fmt.Println("======================================")
			fmt.Println("Column : ", e.column.columnNumber, " Elevator : #", e.elevatorNumber, " Arrived at Requested Floor : ", e.elevatorPosition)
		}
		time.Sleep(300 * time.Millisecond)
		fmt.Println("Column : ", e.column.columnNumber, " Elevator : #", e.elevatorNumber, " Floor : ", e.elevatorPosition)
	}
}

//-------------------------------------"    Doors Status   "-------------------------------------

func (e *Elevator) OpenDoors() {
	fmt.Println("======================================")
	fmt.Println("Doors Opening")
	time.Sleep(1 * time.Second)
	fmt.Println("Doors are Open")
	time.Sleep(1 * time.Second)
	fmt.Println("Button Light Off")
	e.CloseDoors()
}
func (e *Elevator) CloseDoors() {
	if e.doorClearance== true {
		fmt.Println("Doors Closing")
		time.Sleep(1 * time.Second)
		fmt.Println("Doors are Close")
		time.Sleep(1 * time.Second)
		fmt.Println("======================================")
		time.Sleep(1 * time.Second)
	} else if e.doorClearance{
		e.OpenDoors()
	}
}

//-------------------------------------"    Testing Scenario   "-------------------------------------

func main() {
	controller := NewController(1)
	controller.RequestElevator(36)
	controller.RequestElevatorReturning(33, 1)
}