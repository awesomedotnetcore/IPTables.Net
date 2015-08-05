﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPTables.Net.Iptables.U32
{
    public class U32Expression
    {
        private List<IU32Statement> _statements = new List<IU32Statement>();

        public U32Expression(List<IU32Statement> statements)
        {
            _statements = statements;
        }

        public override string ToString()
        {
            return String.Join(" ", _statements.Select((a) => a.ToString()).ToArray());
        }

        public static U32Expression Parse(String strExpr)
        {
            List<IU32Statement> statements = new List<IU32Statement>();
            while (strExpr.Length!=0)
            {
                if (strExpr[0] == '&' && strExpr[1] == '&')
                {
                    statements.Add(U32AndTestStatement.Parse(ref strExpr));
                }
                else
                {
                    statements.Add(U32TestStatement.Parse(ref strExpr));
                }
            }
            return new U32Expression(statements);
        }
    }
}