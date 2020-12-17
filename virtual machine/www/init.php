<?php
        $SERVERNAME="wwwlab.uci.umk.pl";
        $DBNAME="296766_1m9";
        $LOGIN="296766_1m9ad";
        $PASSWORD="hmemyBtftZpY";
        $RESTRICTED_MARK=":";

        $CONNECTION=mysqli_connect($SERVERNAME, $LOGIN, $PASSWORD);
        if($CONNECTION == false) { exit("INIT_DB_1"); }

        $dbSelectErr = mysqli_select_db($CONNECTION, $DBNAME);
        if($dbSelectErr == false) { exit("INIT_DB_2"); }
?>