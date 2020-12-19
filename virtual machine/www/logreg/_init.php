<?php

require "/var/www/logreg/_consts.php";

$CONNECTION=mysqli_connect($SERVERNAME, $LOGIN, $PASSWORD);
if($CONNECTION == false)
{
	exit("INIT_1");
}

$dbSelectErr = mysqli_select_db($CONNECTION, $DBNAME);
if($dbSelectErr == false)
{
	exit("INIT_2");
}

?>
