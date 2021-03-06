     01 CLIRTVO-REC.
        03 MESSAGE-HEADER.
            05 MSGIDA                               PIC  X(030).
            05 MSGLNG                               PIC  9(005).
            05 MSGCNT                               PIC  S9(004)V(4).
            05 FILLER                               PIC  X(010).
            05 MSGID                                PIC  X(010).
        03 MESSAGE-DATA.
            05 BGEN-XXXXX      OCCURS 4.
                07 BGEN-XXXXX-ADDRTYPE         PIC  X(00001).
                07 BGEN-XXXXX-BIRTHP           PIC  X(00020) OCCURS 2.
                07 BGEN-XXXXX-CLTDOBX          PIC  9(008).
                07 FILLER REDEFINES BGEN-XXXXX-CLTDOBX.
                    09 BGEN-XXXXX-CLTDOBX-CCYY      PIC  9(004).
                    09 BGEN-XXXXX-CLTDOBX-MM        PIC  9(002).
                    09 BGEN-XXXXX-CLTDOBX-DD        PIC  9(002).
                07 BGEN-XXXXX-GROUP            OCCURS 3.
                    09 BGEN-XXXXX-GROUP01           PIC X(00001).
                    09 BGEN-XXXXX-GROUP02           PIC S9(005).
                07 BGEN-XXXXX-TRANS-NO1          PIC  9(04) COMP-3. 
                07 BGEN-XXXXX-TRANS-NO2          PIC S9(05) COMP-3. 
                07 BGEN-XXXXX-TRANS-NO3       PIC S9(05)V(03) COMP-3.