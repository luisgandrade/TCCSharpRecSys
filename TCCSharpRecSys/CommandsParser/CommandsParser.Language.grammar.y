%namespace TCCSharpRecSys.CommandsParser
%partial
%parsertype CommandsParserParser
%visibility internal
%tokentype Token
%using TCCSharpRecSys.CommandsParser.Nodes

%union { 
			public int n; 
			public string s;
      public double p;
      public bool b;
      public Script script;
      public IList<IStmt> stmtList;
      public IStmt stmt;
      public IAlgorithm alg;
      public SOM som;
      public KMeans kMeans;
      public CMeans cMeans;
      public Boltzman boltzman;
      public Neighborhood neighborhood;
      public LRate lRate;
	   }

%start main

%token <n> INTEGER
%token <s> STR
%token <p> DOUBLE
%token <b> BOOL
%token <s> METRIC
%token <s> NEIGHBORHOOD
%token SEMICOLON, FOR, OCB, CCB, OSB, CSB, OP, CP, RANGE, COMMA, TRAIN, CLASSIFY, RECOMMEND, USERPROFILES, SETDIR, ATTRCOUNT, BEGIN, SOM, LRATE, KMEANS, CMEANS, 
BOLTZMAN

%type <script> main
%type <stmtList> stmt_list
%type <stmt> stmt
%type <alg> algorithm
%type <som> som
%type <kMeans> kmeans
%type <cMeans> cmeans
%type <boltzman> boltzman
%type <neighborhood> neighborhood
%type <lRate> lrate

%%

main    : BEGIN OCB stmt_list CCB                                                { $$ = new Script(); 
                                                                                    $$.commands = $3;                 }
        ;

stmt_list
        :
          stmt_list stmt SEMICOLON                                               { $1.Add($2);                       }
        | stmt SEMICOLON                                                         { $$ = new List<IStmt>();
                                                                                    $$.Add($1);                       }
        ;

stmt    :
          TRAIN algorithm SEMICOLON                                              { $$ = new Train($2);                }
        | CLASSIFY algorithm SEMICOLON                                           { $$ = new Classify($2);             } 
        | RECOMMEND algorithm SEMICOLON                                          { $$ = new Recommend($2);            }
        | USERPROFILES DOUBLE SEMICOLON                                          { $$ = new BuildUserProfile($2);     }
        | SETDIR STR SEMICOLON                                                   { $$ = new SetDir($2);               }   
        | ATTRCOUNT INTEGER SEMICOLON                                            { $$ = new AttrCount($2);            }
        | FOR OSB INTEGER RANGE INTEGER CSB stmt_list CCB                        { $$ = new For($3, $5, $7);          }
        ;

algorithm
        :
          som                                                                    { $$ = $1; }
        | kmeans                                                                 { $$ = $1; }
        | cmeans                                                                 { $$ = $1; }
        | boltzman                                                               { $$ = $1; }
        ;

som     :
          SOM INTEGER INTEGER METRIC neighborhood lrate BOOL                     { $$ = new SOM($2, $3, new Metric($4), $5, $6, $7); }
        ;

neighborhood
        : NEIGHBORHOOD OP DOUBLE COMMA DOUBLE CP                                 { $$ = new Neighborhood($1, $3, $5); }
        | NEIGHBORHOOD OP DOUBLE CP                                              { $$ = new Neighborhood($1, $3); }
        ;

lrate   :
          LRATE OP DOUBLE COMMA DOUBLE CP                                        { $$ = new LRate($3, $5); }
        | LRATE                                                                  { $$ = new LRate(); }
        ;

kmeans  :
          KMEANS INTEGER                                                          { $$ = new KMeans($2); }
        ;

cmeans  :
          CMEANS INTEGER DOUBLE                                                   { $$ = new CMeans($2, $3); }
        ;

boltzman:
        ;
%%