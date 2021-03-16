<?php 

function DeleteAccount($IDuser)
{
	require "/var/www/logreg/_init.php";
	
	$deleteQuery = "DELETE FROM user WHERE IDuser = '".$IDuser."'";

        if(mysqli_query($CONNECTION, $deleteQuery))
	{
        	return("_SUCCESSFUL");
	}
	return("ADUA_DA_1");
}

function SearchUnverifiedAccounts()
{
	require "/var/www/logreg/_init.php";

	$request = "SELECT IDuser, created FROM user WHERE verified='0';";
	$results = mysqli_query($CONNECTION, $request);
	$currentDate = strtotime(date("Y-m-d H:i:s"));

	while($requestLoop = mysqli_fetch_array($results))
	{
		$accountDate = strtotime($requestLoop['created']);

		if($accountDate + $ADUA_TIME < $currentDate + $TIME_OFFSET)
		{
			$response = DeleteAccount($requestLoop['IDuser']);
			
			if($response != "_SUCCESSFUL")
			{
				echo "\r\n-------------------------\r\n";
				echo "LOGREG ADUA REPORT: ";
				echo date("Y-m-d H:i:s", strtotime("+".$TIME_OFFSET." seconds"));
				echo "\r\nERR: ".$response;
				echo "\r\nID: ".$requestLoop['IDuser'];
				echo " | created: ".$requestLoop['created'];
			        echo "\r\n-------------------------\r\n";
			}
		}
	}
}

SearchUnverifiedAccounts();

?>
