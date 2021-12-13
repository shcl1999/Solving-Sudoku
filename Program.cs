using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CI_PR2_SodokuSolver
{
    class Program
    {
        // IMPORTANT NOTE:
        // WHEN RUNNING: START WITH A NUMBER AND NOT WITH A SPACE!!!

        /* 
          Grid  01
          0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 0 9 0 0 5 0 1 0 3 0 0

          Grid  02
          2 0 0 0 8 0 3 0 0 0 6 0 0 7 0 0 8 4 0 3 0 5 0 0 2 0 9 0 0 0 1 0 5 4 0 8 0 0 0 0 0 0 0 0 0 4 0 2 7 0 6 0 0 0 3 0 1 0 0 7 0 4 0 7 2 0 0 4 0 0 6 0 0 0 4 0 1 0 0 0 3

          Grid  03
          0 0 0 0 0 0 9 0 7 0 0 0 4 2 0 1 8 0 0 0 0 7 0 5 0 2 6 1 0 0 9 0 4 0 0 0 0 5 0 0 0 0 0 4 0 0 0 0 5 0 7 0 0 9 9 2 0 1 0 8 0 0 0 0 3 4 0 5 9 0 0 0 5 0 7 0 0 0 0 0 0

          Grid  04
          0 3 0 0 5 0 0 4 0 0 0 8 0 1 0 5 0 0 4 6 0 0 0 0 0 1 2 0 7 0 5 0 2 0 8 0 0 0 0 6 0 3 0 0 0 0 4 0 1 0 9 0 3 0 2 5 0 0 0 0 0 9 8 0 0 1 0 2 0 6 0 0 0 8 0 0 6 0 0 2 0

          Grid  05
          0 2 0 8 1 0 7 4 0 7 0 0 0 0 3 1 0 0 0 9 0 0 0 2 8 0 5 0 0 9 0 4 0 0 8 7 4 0 0 2 0 8 0 0 3 1 6 0 0 3 0 2 0 0 3 0 2 7 0 0 0 6 0 0 0 5 6 0 0 0 0 8 0 7 6 0 5 1 0 9 0
       */

        static void Main(string[] args)
        {

            string input = Console.ReadLine(); //takes the console input of the given sudoku and turns it into a string
            string[] inputs = input.Split(' '); //takes the string and makes it an array of strings
            int[] getallen = Array.ConvertAll(inputs, int.Parse); //makes the array of strings into an array of ints

            int[,] bord = new int[9, 9]; //create a 9x9 array to keep track of the values of each box
            int teller = 0; //int to serve as a counter for selecting the right value

            //adds the next value in the given sudoku into the list that keeps track of the board
            for (int i = 0; i < 9; i++)  //left to right
            {
                for (int j = 0; j < 9; j++) //up to down
                {
                    bord[i, j] = getallen[teller];
                    teller++;
                }
            }

            //begin timing, because we want to measure the time (of the backtracking algorithm only, not initialization)
            Stopwatch stopwatch = Stopwatch.StartNew(); //creates and start the instance of Stopwatch

            //solveBC(bord) for Chronological Backtracking
            //solveBCFC(bord) for Chronological Backtracking with Forward Checking
            if (solveBCFC(bord)) 
            {
                //stop timing and print execution time of this code
                System.Threading.Thread.Sleep(500);
                stopwatch.Stop();
                long elapsed_time = stopwatch.ElapsedMilliseconds;
                Console.WriteLine("Time elapsed:" + (elapsed_time).ToString() + "ms");
                //print the solution
                printOplossing(bord);
            }
            else //if it can't find a solution (which was never the case lol :) )
                Console.Write("Geen oplossing");
        }

        public static bool solveBC(int[,] bord) //method to solve the chronological backtracking without forward checking
        {
            int row = 0; //var to keep the row
            int col = 0; //var to keep the column

            bool isEmpty = true; //box is standard empty

            for (int i = 0; i < 9; i++) //left to right
            {
                for (int j = 0; j < 9; j++) //up to down
                {
                    if (bord[i, j] == 0) //checks if it is an empty box
                    {
                        row = i; //save this row number for later check
                        col = j; //save this column number for later check

                        isEmpty = false; //box is not empty anymore
                        break;
                    }
                }
                if (!isEmpty) //if the box already contains a number go to the next box and check that
                {
                    break;
                }
            }

            if (isEmpty) //when it's solved, return true
            {
                return true;
            }


            for (int num = 1; num <= 9; num++) //from 1 to 9
            {
                if (isSafe(bord, row, col, num)) //check if the assignation is safe or not
                {
                    bord[row, col] = num; //assign the number to the box
                    if (solveBC(bord)) //calls recursion to go down and eventually either find a solution or return false to choose a different value
                    {
                        return true;
                    }
                    else
                    {
                        bord[row, col] = 0; //if the board wasn't solved, means we assigned a wrong number to it, so reset it and try another option
                    }
                }
            }
            return false;
        }

        public static bool solveBCFC(int[,] bord) //method to initialize chronological backtracking with forward checking
        {
            List<List<int>> domains = new List<List<int>>(); //make list that contains, x y location of the board and also the domains for each box with value 0
            List<int> filled = new List<int>(); //make list that contains the value 0 to assign to the domains for a box that already has a value
            filled.Add(0);


            for (int i = 0; i < 9; i++)  //left to right
            {
                for (int j = 0; j < 9; j++) //up to down
                {
                    if (bord[i, j] == 0) //checks if it's an empty box
                    {
                        List<int> numdomain = new List<int>(); //creates a list to keep track of the domain of the box
                        for (int num = 1; num <= 9; num++) //goes through numbers 1 to 9
                        {
                            if (isSafe(bord, i, j, num)) //if the number is considered safe in the box it is added to the domain of the box
                                numdomain.Add(num);
                        }
                        domains.Add(numdomain); //the domain of the box is added to the list of domains
                    }
                    else
                    {
                        domains.Add(filled); //if the box had a value that is not 0, we give it the domain filled (containing only 0)
                    }
                }
            }


            bool res = BCFChelper(bord, domains); //call the method to solve


            return res;
        }

        public static bool BCFChelper(int[,] bord, List<List<int>> domains) //method to solve chronological backtracking with forward checking
        {
            bool solved = true; //var that checks if the bord is solved or not
            List<int> restoreplaces = new List<int>(); //list that remember which domains of which boxes has changed

            int row = 0; //var to keep the row
            int col = 0; //var to keep the column

            for (int i = 0; i < 9; i++) //left to right
            {
                for (int j = 0; j < 9; j++) //up to down
                {
                    if (bord[i, j] == 0) //checks if it is an empty box
                    {
                        row = i; //save this row number for later check
                        col = j; //save this column number for later check

                        solved = false; //box is not empty anymore
                        break;
                    }
                }
                if (!solved) //if the box already contains a number go to the next box and check that
                {
                    break;
                }
            }

            if (solved) //check if there were no boxes with value 0, means we found a solution
                return true;


            //prepare values for the recursion
            int place = row * 9 + col; //var for the place of a box in the list
            List<int> options = domains[place]; //possible values for this box

            for (int op = 0; op < options.Count(); op++) //goes through the possible values for this box
            {
                int getal = options[op]; //value that is being tested
                bord[row, col] = getal; //set value for box to the var getal
                //remove element from domains
                //look if there are domains empty or not
                Tuple<List<List<int>>, List<int>> tuple = (DeleteDomain(bord, getal, place, domains)); //set the var tuple to the tuple after DeleteDomain has removed elements
                domains = tuple.Item1;
                restoreplaces = tuple.Item2;

                if (BCFChelper(bord, domains)) //recursion and make all domains smaller
                {
                    return true;
                }
                else
                {
                    bord[row, col] = 0; //if the board wasn't solved, means we assigned a wrong number to it, so reset it and try another option
                    domains = RestoreDomain(restoreplaces, getal, domains); //restore values in the domain
                }
            }

            return false;
        }

        public static Tuple<List<List<int>>, List<int>> DeleteDomain(int[,] bord, int item, int place, List<List<int>> domains) //method to remove an element from a box's domain
        {
            List<int> restoreplaces = new List<int>(); //list that marks down the places that have been updated
            //to check the row the box is in
            bool isnotempty = true; //check if a list is empty or not after removing
            for (int a = 1; place + a < (place / 9 + 1) * 9; a++) //for all domains from the same row
            {
                if (domains[place + a].Contains(item)) //if the box's domain contains the element needed to be removed
                {
                    (domains[place + a]).Remove(item); //remove the element from the box's domain
                    restoreplaces.Add((place + a)); //mark the place as having been updated
                    if ((domains[place + a]).Count() == 0) //checks if the domain of the box is empty or not
                    {
                        isnotempty = false;
                        break; //if the box's domain is empty: break, because no solution possible
                    }
                }
            }
            //to check the column the box is in
            if (isnotempty == true) //if it's false, no need to check
            {
                int a = 9; //only go through the domains of the boxes lower in the column, so never in row 0
                while (place + a < 81) //as long as it hasn't gone through all boxes
                {
                    if (domains[place + a].Contains(item)) //if the box's domain contains the element needed to be removed
                    {
                        (domains[place + a]).Remove(item); //remove the element from the box's domain
                        restoreplaces.Add((place + a)); //mark the place as having been updated
                        if ((domains[place + a]).Count() == 0) //checks if the domain of the box is empty or not
                        {
                            isnotempty = false;
                            break; //if the box's domain is empty: break, because no solution possible
                        }
                    }
                    a += 9; //to only go through the domains of the column of the box
                }
            }
            //to check the block the box is in
            if (isnotempty == true) //if it's false, no need to check
            {
                int row = place / 9; //var to keep track of the row
                int col = place % 9; //var to keep track of the column
                int rowStart = row - row % 3; //var to find what 3x3 block the box is in
                int colStart = col - col % 3; //var to find what 3x3 block the box is in

                for (int i = rowStart; i < rowStart + 3; i++) //for every box in the 3x3 block
                    for (int j = colStart; j < colStart + 3; j++)
                    {
                        int check = i * 9 + j; //var to see if it's further down or right than the box
                        if (check > place) //check to see if it's further down or right than the box
                        {
                            if (domains[check].Contains(item)) //if the box's domain contains the element needed to be removed
                            {
                                (domains[check]).Remove(item); //remove the element from the box's domain
                                restoreplaces.Add((check)); //mark the place as having been updated
                                if ((domains[check]).Count() == 0) //checks if the domain of the box is empty or not
                                {
                                    isnotempty = false; //if the box's domain is empty: break, because no solution possible
                                    break;
                                }
                            }
                        }
                    }


            }

            Tuple<List<List<int>>, List<int>> res = new Tuple<List<List<int>>, List<int>>(domains, restoreplaces); //the result after DeleteDomain has gone over the list
            return res;
        }

        public static List<List<int>> RestoreDomain(List<int> restoreplaces, int item, List<List<int>> domains) //method to restore elements from domains
        {
            //restore elements from domains
            foreach (int place in restoreplaces) //for every box's domain that has been updated in DeleteDomain
            {
                domains[place].Add(item); //add back the element removed
                domains[place].Sort(); //sort the list of elements
            }

            return domains; //return the list of domains after it has restored a number to the domains it had been removed from
        }

        public static bool isSafe(int[,] board, int row, int col, int num) //method to test if a value is consistent with the sudoku rules in a given box
        {
            //check if the number is unique in a row
            for (int i = 0; i < 9; i++)
                if (board[row, i] == num) //if the number already occured, return false
                    return false;

            //check if the number is unique in a column 
            for (int i = 0; i < 9; i++)
                if (board[i, col] == num) //if the number already occured, return false
                    return false;

            //check if it already contains in a block.
            int rowStart = row - row % 3;
            int colStart = col - col % 3;

            for (int i = rowStart; i < rowStart + 3; i++)
                for (int j = colStart; j < colStart + 3; j++)
                    if (board[i, j] == num)
                        return false; //not 'safe'

            //if no double numbers occurred, then it's 'safe'
            return true;
        }

        public static void printOplossing(int[,] bord)//method to print the solved board
        {
            for (int i = 0; i < 9; i++) //left to right
            {
                for (int j = 0; j < 9; j++) //up to down
                {
                    Console.Write(string.Format("{0} ", bord[i, j]));
                }
                Console.Write(Environment.NewLine);
            }
            Console.ReadLine();
        }

    }
}