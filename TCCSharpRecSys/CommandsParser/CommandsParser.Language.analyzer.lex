%namespace TCCSharpRecSys.CommandsParser
%scannertype CommandsParserScanner
%visibility internal
%tokentype Token

%option stack, minimize, parser, verbose, persistbuffer, noembedbuffers 

Eol             (\r\n?|\n)
NotWh           [^ \t\r\n]
Space           [ \t]
Integer          [0-9]+
Double      ([0-9]*|(0?\.[0-9]+)) 
Str             [a-z][a-zA-Z0-9_-]*
%{

%}

%%

/* Scanner body */

{Space}+		/* skip */

"begin"                { return (int) Token.BEGIN;                                        }
"{"                    { return (int) Token.OCB;                                          }
"}"                    { return (int) Token.CCB;                                          }
";"                    { return (int) Token.SEMICOLON;                                    }
"["                    { return (int) Token.OSB;                                          }
"]"                    { return (int) Token.CSB;                                          }
"("                    { return (int) Token.OP;                                           }
")"                    { return (int) Token.CP;                                           }
"\.\."                 { return (int) Token.RANGE;                                        }
","                    { return (int) Token.COMMA;                                        } 
"for"                  { return (int) Token.FOR;                                          }               
"train"                { return (int) Token.TRAIN;                                        }
"classify"             { return (int) Token.CLASSIFY;                                     }
"recommend"            { return (int) Token.RECOMMEND;                                    }
"build_profiles"       { return (int) Token.USERPROFILES;                                 }
"set_dir"              { return (int) Token.SETDIR;                                       }
"attr_count"           { return (int) Token.ATTRCOUNT;                                    }
"euclidian"            { yylval.s = yytext; return (int) Token.METRIC;                    }
"manhattan"            { yylval.s = yytext; return (int) Token.METRIC;                    }
"cosine"               { yylval.s = yytext; return (int) Token.METRIC;                    }
"som"                  { return (int) Token.SOM;                                          }
"gaussian"             { yylval.s = yytext; return (int) Token.NEIGHBORHOOD;              }
"lrate"                { return (int) Token.LRATE;                                        }
"kmeans"               { return (int) Token.KMEANS;                                       }
"cmeans"               { return (int) Token.CMEANS;                                       }
"boltzman"             { return (int) Token.BOLTZMAN;                                     }
"true"                 { yylval.b = true; return (int) Token.BOOL;                        }         
"false"                { yylval.b = false; return (int) Token.BOOL;                       }         
{Integer}              { yylval.n = int.Parse(yytext); return (int) Token.INTEGER;        }
{Double}               { yylval.p = double.Parse(yytext); return (int) Token.DOUBLE;      }
{Str}                  { yylval.s = yytext; return (int) Token.STR;                       }



%%