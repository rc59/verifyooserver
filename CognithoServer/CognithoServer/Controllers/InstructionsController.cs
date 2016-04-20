using CognithoServer.Models;
using CognithoServer.Objects;
using CognithoServer.Utils;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CognithoServer.Controllers
{
    public class InstructionsController : ApiController
    {
        [HttpGet]
        public InstructionsList Get()
        {
            new UtilsLicense().CheckAppKey();
            MongoCollection<Instruction> instructions = new UtilsDB().GetCollectionInstructions();

            List<Instruction> tempList = new List<Instruction>();

            //foreach (Instruction instruction in instructions.FindAll().SetLimit(UtilsConfig.INSTRUCTIONS_IN_TEMPLATE))
            foreach (Instruction instruction in instructions.FindAll().SetLimit(UtilsConfig.INSTRUCTIONS_IN_TEMPLATE))
            {
                tempList.Add(instruction);
            }
            InstructionsList instructionsList = 
                new InstructionsList(tempList, UtilsConfig.INSTRUCTIONS_FOR_AUTH, UtilsConfig.NUM_FUTILITY_INSTRUCTIONS);

            return instructionsList;

            List<Instruction> list = new List<Instruction>();
            int[] randomVector = new UtilsCalc().GenerateRandomVector(tempList.Count);

            for (int idx = 0; idx < UtilsConfig.INSTRUCTIONS_IN_TEMPLATE; idx++)
            {
                list.Add(tempList[randomVector[idx]]);
            }

            instructionsList =
                new InstructionsList(list, UtilsConfig.INSTRUCTIONS_FOR_AUTH, UtilsConfig.NUM_FUTILITY_INSTRUCTIONS);
            return instructionsList;
        }

        [HttpGet]
        public List<Instruction> Get(string userId, bool getAll = false)
        {
            UtilsDB utilsDB = new UtilsDB();
            UtilsLicense utilsLicense = new UtilsLicense();
            UtilsData utilsData = new UtilsData();

            utilsLicense.CheckAppKey();
            List<Instruction> list = new List<Instruction>();
            Template template = utilsDB.GetTemplateByUsername(userId);

            if (template == null)
            {
                new UtilsErrorHandling().ReturnBadRequest(Resources.Resources.errUserNotFound, ConstsErrorCodes.ERROR_CODE_3);
                return new List<Instruction>();
            }

            Dictionary<int, bool> dictInstructionIdxs = utilsData.GetInstructionIdxsHash(template);

            MongoCollection<Instruction> instructions = utilsDB.GetCollectionInstructions();            

            bool isInstructionInUserTemplate;            

            if (getAll)
            {
                foreach (Instruction instruction in instructions.FindAll())
                {
                    isInstructionInUserTemplate =
                        dictInstructionIdxs.ContainsKey(instruction.InstructionIdx) &&
                        list.Count < UtilsConfig.INSTRUCTIONS_IN_TEMPLATE;

                    if (isInstructionInUserTemplate)
                    {
                        list.Add(instruction);
                    }
                }
            }
            else
            {
                //List<GestureUniqueFactor> uniqueFactors = new UtilsTemplate().GetTemplateUniqueFactors(template);

                //Dictionary<int, bool> dictInstructionsForAuth = new Dictionary<int, bool>();

                //for(int idx = 0; idx < UtilsConfig.INSTRUCTIONS_FOR_AUTH; idx++)
                //{
                //    if(!dictInstructionsForAuth.ContainsKey(uniqueFactors[idx].InstructionIdx))
                //    {
                //        dictInstructionsForAuth.Add(uniqueFactors[idx].InstructionIdx, true);
                //    }
                //}

                int[] vector = new int[UtilsConfig.INSTRUCTIONS_IN_TEMPLATE];
                for(int idx = 0; idx < vector.Length; idx++)
                {
                    vector[idx] = idx;
                }

                Random random = new Random();
                int randomNumber, temp;

                for (int idx = 0; idx < UtilsConfig.INSTRUCTIONS_IN_TEMPLATE; idx++)
                {
                    randomNumber = random.Next(0, UtilsConfig.INSTRUCTIONS_IN_TEMPLATE - 1);
                    temp = vector[idx];
                    vector[idx] = vector[randomNumber];
                    vector[randomNumber] = temp;
                }

                Instruction[] listInstructions = new Instruction[8];
                int count = 0;
                foreach (Instruction instruction in instructions.FindAll())
                {
                    //isInstructionInUserTemplate =
                    //    dictInstructionIdxs.ContainsKey(instruction.InstructionIdx) &&
                    //    dictInstructionsForAuth.ContainsKey(instruction.InstructionIdx) &&
                    //    list.Count < UtilsConfig.INSTRUCTIONS_FOR_AUTH;

                    listInstructions[count] = instruction;
                    count++;
                    //if (isInstructionInUserTemplate)
                    //{
                    //    list.Add(instruction);
                    //}
                }

                int countFutility = 0;
                for(int idx = 0; idx < listInstructions.Length; idx++)
                {
                    if(template.Gestures[listInstructions[vector[idx]].InstructionIdx].IsInTemplate)
                    {
                        list.Add(listInstructions[vector[idx]]);
                    }
                    else
                    {
                        if(countFutility < UtilsConfig.NUM_FUTILITY_INSTRUCTIONS)
                        {
                            countFutility++;
                            list.Add(listInstructions[vector[idx]]);
                        }
                    }

                    if(list.Count >= (UtilsConfig.INSTRUCTIONS_FOR_AUTH + UtilsConfig.NUM_FUTILITY_INSTRUCTIONS - 1))
                    {
                        break;
                    }
                }
            }

            return list;
        }
    }
}
