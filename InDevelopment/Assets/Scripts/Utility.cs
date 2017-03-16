﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility {

	public static T[] shuffleArray<T>(T[] array, int seed)
    {
        System.Random psudoRand = new System.Random(seed);
        for(int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = psudoRand.Next(i, array.Length);
            T temp = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = temp;
        }

        return array;
    }
}
