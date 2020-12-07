<?php
       /*
         * E1 -> Server authorization error
         * E2 -> Database error
        */
        $SERVERNAME="wwwlab.uci.umk.pl";
        $DBNAME="296766_1m9";
        $LOGIN="296766_1m9ad";
        $PASSWORD="hmemyBtftZpY";

        $CONNECTION=mysqli_connect($SERVERNAME, $LOGIN, $PASSWORD);
        if($CONNECTION == false) {
                exit("E1");
        }

        $dbSelectErr = mysqli_select_db($CONNECTION, $DBNAME);
        if($dbSelectErr == false) {
                exit("E2");
        }
?>