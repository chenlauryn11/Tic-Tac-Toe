using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class q : MonoBehaviour
{
    //Returns a Random Number as a float in the Range 
    public static float getRandF(float min, float max)
    {
        //Return a random float
        return Random.Range(min, max);
    }

    //Returns a Random Number as an int in the Range
    public static int getRandI(float min, float max)
    {
        //Return a random int
        return (int)getRandF(min, max);
    }

    //Return whether the number is within the minimum and maximum values
    public static bool inRange(float min, float max, float num)
    {
        return num <= max && num >= min;
    }

    public static bool inArr(int[] arr, int num)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == num)
            {
                return true;
            }
        }

        return false;
    }

    //Prints the board in the console
    public static void printBoard(int[] board)
    {
        string str = "";

        for (int i = 0; i < board.Length; i++)
        {
            str += board[i] + ", ";
        }

        Debug.Log("Board: " + str);
    }

    public static int[] copyList(List<int> l)
    {
        int[] result = new int[l.Count];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = l[i];
        }

        return result;
    }

    public static int[] copyArr(int[] b)
    {
        int[] result = new int[b.Length];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = b[i];
        }

        return result;
    }


    //Shuffles the contents of the array
    public static int[] randomize(int[] arr)
    {
        List<int> list = arrToList(arr);

        int[] result = new int[arr.Length];

        int index = 0;

        while (list.Count > 0)
        {
            int rand = getRandI(0, list.Count - 1);
            result[index] = list[rand];
            list.RemoveAt(rand);
            index++;
        }

        return result;
    }

    public static List<int> arrToList(int[] arr)
    {
        List<int> result = new List<int>();

        for (int i = 0; i < arr.Length; i++)
        {
            result.Add(arr[i]);
        }

        return result;
    }
}
