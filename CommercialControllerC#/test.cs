using System;
using System.Collections.Generic;
using System.Threading;

namespace Commercial_controller
{
    //this corporate elevator algorithme have been made in C#

    //the Main function to enter your value are at the bottom

    //RequestElevator and Assignelevator

    //FloorNumber = the place where the customer is at
    //RequestedFloor = the floor the customer want to go


    // this class is where the request from use is take and make it work

    public class ElevatorController
    {
        public int no_of_floor;
        public int no_of_elevator_per_column;
        public int no_of_column;
        public string user_direction;
        public Battery battery;

        public ElevatorController(int no_of_floor, int no_of_column, int no_of_elevator_per_column, string user_direction)
        {
            this.no_of_floor = no_of_floor;
            this.no_of_column = no_of_column;
            this.no_of_elevator_per_column = no_of_elevator_per_column;
            this.user_direction = user_direction;
            this.battery = new Battery(this.no_of_column);


        }

        // here is the request made by people that want to go down
        public Elevator RequestElevator(int FloorNumber, int RequestedFloor)
        {
            Thread.Sleep(200);
            Console.WriteLine("Request elevator to floor : " + FloorNumber);
            Console.WriteLine("---------------------------------------------------");
            Thread.Sleep(200);
            Console.WriteLine("Call Button Light On");
            Console.WriteLine("---------------------------------------------------");
            var column = battery.Find_best_column(FloorNumber);
            user_direction = "down";
            var elevator = column.Find_Requested_elevator(FloorNumber, user_direction);
            if (elevator.elevator_floor > FloorNumber)
            {
                elevator.Send_request(FloorNumber, column.column_no);
                elevator.Send_request(RequestedFloor, column.column_no);
            }

            else if (elevator.elevator_floor < FloorNumber)
            {
                elevator.Move_down(RequestedFloor, column.column_no);
                elevator.Send_request(FloorNumber, column.column_no);
                elevator.Send_request(RequestedFloor, column.column_no);
            }
            Console.WriteLine("Button Light Off");

            return elevator;
        }

        // here is the request from people that want to go up to a floor X
        public Elevator AssignElevator(int RequestedFloor)
        {
            Thread.Sleep(200);
            Console.WriteLine("Requested floor : " + RequestedFloor);
            Thread.Sleep(200);
            Console.WriteLine("Call Button Light On");


            Column column = battery.Find_best_column(RequestedFloor);
            user_direction = "up";
            var FloorNumber = 1;
            Elevator elevator = column.Find_Assign_elevator(RequestedFloor, FloorNumber, user_direction);

            elevator.Send_request(FloorNumber, column.column_no);

            elevator.Send_request(RequestedFloor, column.column_no);



            return elevator;
        }

    }

    // here is where all operation are made door/movement
    public class Elevator
    {
        public int elevator_no;
        public string status;
        public int elevator_floor;
        public string elevator_direction;
        public bool Sensor;
        public int FloorDisplay;
        public List<int> floor_list;

        public Elevator(int elevator_no, string status, int elevator_floor, string elevator_direction)
        {
            this.elevator_no = elevator_no;
            this.status = status;
            this.elevator_floor = elevator_floor;
            this.elevator_direction = elevator_direction;
            this.FloorDisplay = elevator_floor;
            this.Sensor = true;
            this.floor_list = new List<int>();
        }

        // sendrequest receive information that people made in the requestelevator and assign elevator
        public void Send_request(int RequestedFloor, char column_no)
        {
            floor_list.Add(RequestedFloor);
            if (RequestedFloor > elevator_floor)
            {
                floor_list.Sort((a, b) => a.CompareTo(b));
            }
            else if (RequestedFloor < elevator_floor)
            {
                floor_list.Sort((a, b) => -1 * a.CompareTo(b));

            }

            Operate_elevator(RequestedFloor, column_no);
        }

        // here is where the task are separate depending on the direction 
        public void Operate_elevator(int RequestedFloor, char column_no)
        {
            if (RequestedFloor == elevator_floor)
            {
                Open_door();
                this.status = "moving";

                this.floor_list.Remove(0);
            }
            else if (RequestedFloor < this.elevator_floor)
            {
                status = "moving";
                Console.WriteLine("Button Light Off");
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("Column : " + column_no + " Elevator : " + this.elevator_no + " " + status);
                Console.WriteLine("---------------------------------------------------");
                this.elevator_direction = "down";
                Move_down(RequestedFloor, column_no);
                this.status = "stopped";
                Console.WriteLine("Column : " + column_no + " Elevator : " + this.elevator_no + " " + status);

                this.Open_door();
                this.floor_list.Remove(0);
            }
            else if (RequestedFloor > this.elevator_floor)
            {
                Thread.Sleep(300);
                this.status = "moving";
                Console.WriteLine("Button Light Off");
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("Column : " + column_no + " Elevator : " + this.elevator_no + " " + status);
                Console.WriteLine("---------------------------------------------------");
                this.elevator_direction = "up";
                this.Move_up(RequestedFloor, column_no);
                this.status = "stopped";
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("Column : " + column_no + " Elevator : " + this.elevator_no + " " + status);


                this.Open_door();

                this.floor_list.Remove(0);
            }

        }
        // Open And Close Door
        public void Open_door()
        {
            Thread.Sleep(300);

            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("Door is Opening");

            Thread.Sleep(300);
            Console.WriteLine("Door is Open");
            Thread.Sleep(300);

            this.Close_door();
        }
        public void Close_door()
        {
            if (Sensor == true)
            {
                Console.WriteLine("Door is Closing");
                Thread.Sleep(300);
                Console.WriteLine("Door is Close");
                Thread.Sleep(300);


                Console.WriteLine("---------------------------------------------------");
            }
            else if (Sensor == false)
            {
                Open_door();
            }
        }

        // here is the moving direction

        public void Move_up(int RequestedFloor, char column_no)
        {
            Console.WriteLine("Column : " + column_no + " Elevator : #" + elevator_no + "  Current Floor : " + this.elevator_floor);
            Thread.Sleep(300);
            Console.WriteLine("---------------------------------------------------");
            while (this.elevator_floor != RequestedFloor)
            {
                this.elevator_floor += 1;
                Console.WriteLine("Column : " + column_no + " Elevator : #" + elevator_no + "  Floor : " + this.elevator_floor);

                Thread.Sleep(100);
            }
        }

        public void Move_down(int RequestedFloor, char column_no)
        {
            Console.WriteLine("Column : " + column_no + " Elevator : #" + elevator_no + "  Current Floor : " + this.elevator_floor);
            Thread.Sleep(300);
            Console.WriteLine("---------------------------------------------------");

            while (this.elevator_floor != RequestedFloor)
            {
                this.elevator_floor -= 1;
                Console.WriteLine("Column : " + column_no + " Elevator : #" + elevator_no + "  Floor : " + this.elevator_floor);

                Thread.Sleep(100);
            }
            Console.WriteLine("---------------------------------------------------");

        }

    }

    // here is where the column redirect task to find the best column and create my elevator

    public class Column
    {
        public char column_no;
        public int no_of_floor;
        public int no_of_elevator_per_column;
        public List<Elevator> elevator_list;
        public List<int> call_button_list;


        public Column(char column_no, int no_of_floor, int no_of_elevator_per_column)
        {
            this.column_no = column_no;
            this.no_of_floor = no_of_floor;
            this.no_of_elevator_per_column = no_of_elevator_per_column;
            elevator_list = new List<Elevator>();
            call_button_list = new List<int>();
            for (int i = 0; i < this.no_of_elevator_per_column; i++)
            {
                Elevator elevator = new Elevator(i, "idle", 1, "up");
                elevator_list.Add(elevator);
            }
        }

        // here is the find best elevator for the guys request to go up

        public Elevator Find_Assign_elevator(int RequestedFloor, int FloorNumber, string user_direction)
        {

            foreach (var elevator in elevator_list)
                if (elevator.status == "idle")
                {
                    return elevator;
                }

            var bestElevator = 0;
            var shortest_distance = 1000;
            for (var i = 0; i < this.elevator_list.Count; i++)
            {
                var ref_distance = Math.Abs(elevator_list[i].elevator_floor - elevator_list[i].floor_list[0]) + Math.Abs(elevator_list[i].floor_list[0] - 1);
                if (shortest_distance >= ref_distance)
                {
                    shortest_distance = ref_distance;
                    bestElevator = i;
                }
            }
            return elevator_list[bestElevator];
        }



        // here is the find best elevator for the guys want to go down

        public Elevator Find_Requested_elevator(int RequestedFloor, string user_direction)
        {
            var shortest_distance = 999;
            var bestElevator = 0;

            for (var i = 0; i < this.elevator_list.Count; i++)
            {
                var ref_distance = elevator_list[i].elevator_floor - RequestedFloor;

                if (ref_distance > 0 && ref_distance < shortest_distance)
                {
                    shortest_distance = ref_distance;
                    bestElevator = i;
                }
            }
            return elevator_list[bestElevator];
        }

    }
    // here is the battery the things that create my column so that my elevator can work 
    public class Battery
    {
        public string battery_status;
        public int no_of_column;
        public List<Column> column_list;
       

        public Battery(int no_of_column)
        {
            this.no_of_column = no_of_column;
            this.battery_status = "on";
            column_list = new List<Column>();



            char cols = 'A';
            for (int i = 0; i < this.no_of_column; i++, cols++)
            {

                Column column = new Column(cols, 85, 5);

                column.column_no = cols;
                column_list.Add(column);

            }
        }
        // here is where the best column are find
        public Column Find_best_column(int RequestedFloor)
        {
            Column best_column = null;
            foreach (Column column in column_list)
            {
                if (RequestedFloor > 1 && RequestedFloor <= 6 || RequestedFloor == 7)
                {
                    best_column = column_list[0];
                }
                else if (RequestedFloor > 6 && RequestedFloor <= 26 || RequestedFloor == 7)
                {

                    best_column = column_list[1];


                }
                else if (RequestedFloor > 26 && RequestedFloor <= 46 || RequestedFloor == 7)
                {
                    best_column = column_list[2];


                }
                else if (RequestedFloor > 46 && RequestedFloor <= 66 || RequestedFloor == 7)
                {
                    best_column = column_list[3];


                }

            }
            return best_column;
        }
    }

}



    