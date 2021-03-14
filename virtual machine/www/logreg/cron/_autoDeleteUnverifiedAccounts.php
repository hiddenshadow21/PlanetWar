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

	echo "\r\n-------------------------\r\n";
	echo "LOGREG ADUA REPORT: ";	
	echo date("Y-m-d H:i:s", strtotime("+".$TIME_OFFSET." seconds")); 

	$currentDate = strtotime(date("Y-m-d H:i:s"));

	while($requestLoop = mysqli_fetch_array($results))
	{
		echo "\r\nID: ".$requestLoop['IDuser'];

		$accountDate = strtotime($requestLoop['created']);
  
  		echo " | created: ".$requestLoop['created'];
		
		if($accountDate + $ADUA_TIME < $currentDate + $TIME_OFFSET)
		{
			$response = DeleteAccount($requestLoop['IDuser']);
			if($response == "_SUCCESSFUL")
			{
				echo " >> ACCOUNT DELETED!";
			}
			else
			{
				echo " >> ERR: ".$response;
			}
		}
		else
		{
			$diff = round(($accountDate + $ADUA_TIME - $currentDate + $TIME_OFFSET) / 60 / 60);
			echo " | remaining ".$diff."hours";
		}
	}
	echo "\r\n-------------------------\r\n";
}

SearchUnverifiedAccounts();

?>
