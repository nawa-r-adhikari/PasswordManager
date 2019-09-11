using System;
using System.Security.Cryptography;

namespace PasswordManager
{

    // ===============================
    // AUTHOR     : NAWA ADHIKARI
    // CREATED DATE     : 15 FEB 2019
    // PURPOSE     : RandomPasswordGenerator class for iD Password Manager
    //              Generates password based on length of the password and parameters passed
    // SPECIAL NOTES:
    // ===============================
    // Change History:
    // Added Password validation
    //
    //==================================
    public class RandPasswordGenerator
    {
        //Method Generate takes password length, lower case, upper case, 
        //numbers and special characters as parameters
        public static string Generate(int pwdLength, string lCase, string uCase, string num, string spec)
        {
            // Checks if the length of the password is zero or less.
            if (pwdLength <= 0)
                return null;


            //creates local array grouped by type of characters
            //this makes the password strength strong.
            char[][] charGroups = new char[][]
            {
            lCase.ToCharArray(),
            uCase.ToCharArray(),
            num.ToCharArray(),
            spec.ToCharArray()
            };

            //stores what character is left in the group
            int[] charsLeftInGroup = new int[charGroups.Length];


            //copies all characters to the CharsLeft group
            for (int i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;
 
            //this array is used to iterate through the unused character groups
            int[] leftGroupsOrder = new int[charGroups.Length];

            // Initially, all character groups are not used.
            for (int i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;


            //Seed will be created by random number generator
            //4 byte of random bytes will be used as seed and converted to integer value
            byte[] randomBytes = new byte[4];

            // Cryptographic random number generator will be used to 
            // generate 4 random bytes.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            //conversion to 32 bit integer from 4 byte
            int seed = BitConverter.ToInt32(randomBytes, 0);

            //Uses random number as seed
            Random random = new Random(seed);

            //holds the password character initialized to null
            char[] password = null;

            //declares array length
            password = new char[pwdLength];

            // next char Inde is kept track of
            int nextCharIdx;

            // next character group index is kept track of.
            int nextGroupIdx;

            //not used character group will be tracked using nextLeftGroupsOrderIdx
            int nextLeftGroupsOrderIdx;

            //non processed character in a group will be tracked by lastCharIdx
            int lastCharIdx;

            //keeps track of last non-processed group
            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            // generates password one character at a time
            for (int i = 0; i < password.Length; i++)
            {

                //Picks random character until a character group remained is unprocessed.
                if (lastLeftGroupsOrderIdx == 0)
                    nextLeftGroupsOrderIdx = 0;
                else
                    nextLeftGroupsOrderIdx = random.Next(0,
                                                         lastLeftGroupsOrderIdx);

                // Get the actual index of the character group, from which 
                //next character is picked
                nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                //index of the last unprocessed characters in the group is tracked
                lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;


                //gets random character from ununsed character group until one character is left
                if (lastCharIdx == 0)
                    nextCharIdx = 0;
                else
                    nextCharIdx = random.Next(0, lastCharIdx + 1);

                // Adds character to the password.
                password[i] = charGroups[nextGroupIdx][nextCharIdx];

                //once last character is process in the group it will start over
                if (lastCharIdx == 0)
                    charsLeftInGroup[nextGroupIdx] =
                                              charGroups[nextGroupIdx].Length;
                // If there are more unprocessed characters left.
                else
                {
                    // Swap processed character with the last unprocessed character
                    // so that we don't pick it until we process all characters in
                    // this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] =
                                    charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }
                    // Decrement the number of unprocessed characters in
                    // this group.
                    charsLeftInGroup[nextGroupIdx]--;
                }

                // If we processed the last group, start all over.
                if (lastLeftGroupsOrderIdx == 0)
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                // There are more unprocessed groups left.
                else
                {
                    // Swap processed group with the last unprocessed group
                    // so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                    leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    // Decrement the number of unprocessed groups.
                    lastLeftGroupsOrderIdx--;
                }
            }

            // Convert password characters into a string and return the result.
            return new string(password);
        }


    }
}
