﻿using PdfTextReader.TextStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.Parser
{
    class TransformArtigo2 : IAggregateStructure<TextSegment, Artigo>
    {
        public bool Aggregate(TextSegment line)
        {
            return false;
        }

        public Artigo Create(List<TextSegment> segments)
        {
            TextSegment segment = segments[0];
            string hierarchy = null;
            string titulo = null;
            string body = null;
            string caput = null;
            List<string> resultProcess = new List<string>() { null, null, null };


            //Definindo Titulo e hierarquia
            int idxTitle = segment.Title.Count() - 1;

            if (idxTitle == 0)
            {
                titulo = segment.Title[0].Text;
            }
            else if (idxTitle > 0)
            {
                for (int i = 0; i < segment.Title.Count() - 1; i++)
                {
                    hierarchy = hierarchy + segment.Title[i].Text + ":";
                }
                titulo = segment.Title[idxTitle].Text;
            }

            //Definindo Caput
            if (segment.Body[0].TextAlignment == TextAlignment.RIGHT && segment.Body[1].TextAlignment == TextAlignment.JUSTIFY)
                caput = segment.Body[0].Text;


            //Definindo Body
            int idxBody = segment.Body.ToList().FindIndex(s => s.TextAlignment == TextAlignment.JUSTIFY);

            body = segment.Body[idxBody].Text;

        
            //Definindo Assinatura, Cargo e Data
            int idxSigna = segment.Body.ToList().FindIndex(s => s.TextAlignment == TextAlignment.JUSTIFY) + 1;
            
            if (idxSigna > 0 && idxSigna < segment.Body.Count())
            {
                resultProcess.Clear();
                resultProcess = ProcessSignatureAndRole(segment.Body[idxSigna].Lines);
            }

            return new Artigo()
            {
                Hierarquia = hierarchy,
                Titulo = titulo,
                Caput = caput,
                Corpo = body,
                Assinatura = resultProcess[0],
                Cargo = resultProcess[1],
                Data = resultProcess[2]
            };
        }


        List<string> ProcessSignatureAndRole(List<TextLine> lines)
        {

            string signature = null;
            string role = null;
            string date = null;


            foreach (var item in lines)
            {
                if (item.Text.ToUpper() == item.Text)
                {
                    signature = signature + "\n" + item.Text;
                }
                else if (item.FontStyle == "Italic")
                {
                    signature = signature + "\n" + item.Text;
                }
                else if (item.Text.All(Char.IsDigit))
                {
                    date = item.Text;
                }
                else
                {
                    role = role + "\n" + item.Text;
                }
            }

            return new List<string>() { signature, role, date };
        }

        public void Init(TextSegment line)
        {
           
        }
    }
}
