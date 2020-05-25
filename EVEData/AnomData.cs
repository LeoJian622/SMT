﻿//-----------------------------------------------------------------------
// EVE Anom Data
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace SMT.EVEData
{
    /// <summary>
    /// The Per system Anom Data
    /// </summary>
    public class AnomData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnomData" /> class
        /// </summary>
        public AnomData()
        {
            Anoms = new SerializableDictionary<string, Anom>();
        }

        /// <summary>
        /// Gets or sets the Anom signature to data dictionary
        /// </summary>
        public SerializableDictionary<string, Anom> Anoms { get; }

        /// <summary>
        /// Gets or sets the name of the System this AnomData is for
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Update the AnomData from the string (usually the clipboard)
        /// </summary>
        /// <param name="pastedText">raw Anom strings</param>
        public void UpdateFromPaste(string pastedText)
        {
            bool validPaste = false;
            List<string> itemsToKeep = new List<string>();
            string[] pastelines = pastedText.Split('\n');
            foreach (string line in pastelines)
            {
                // split on tabs
                string[] words = line.Split('\t');
                if (words.Length == 6)
                {
                    // only care about "Cosmic Anomaly" 
                    if (words[1] == "Cosmic Anomaly")
                    {
                        validPaste = true;

                        string sigID = words[0];
                        string sigType = words[2];
                        string sigName = words[3];

                        itemsToKeep.Add(sigID);

                        // valid sig
                        if (Anoms.Keys.Contains(sigID))
                        {
                            // updating an existing one
                            Anom an = Anoms[sigID];
                            if (an.Type == Anom.SignatureType.Unknown)
                            {
                                an.Type = Anom.GetTypeFromString(sigType);
                            }

                            if (sigName != string.Empty)
                            {
                                an.Name = sigName;
                            }
                        }
                        else
                        {
                            Anom an = new Anom();
                            an.Signature = sigID;

                            if (sigType != string.Empty)
                            {
                                an.Type = Anom.GetTypeFromString(sigType);
                            }

                            if (sigName != string.Empty)
                            {
                                an.Name = sigName;
                            }

                            Anoms.Add(sigID, an);
                        }
                    }
                }
            }

            // if we had a valid paste dump any items we didnt reference, brute force scan and remove.. come back to this later..
            if (validPaste)
            {
                List<string> toRemove = new List<string>();
                foreach (string an in Anoms.Keys.ToList())
                {
                    if (!itemsToKeep.Contains(an))
                    {
                        toRemove.Add(an);
                    }
                }

                foreach (string s in toRemove)
                {
                    Anoms.Remove(s);
                }
            }
        }
    }
}