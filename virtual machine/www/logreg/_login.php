<?php

$email = "";

function DownloadDataFromUnity()
{
	global $email;

	if(isset($_POST["email"]))
	{
		$email = $_POST["email"];
	}
	else
	{
		return("L_DDFU_1");
	}
	
	return("_SUCCESSFUL");
}

function AuthorizeUser()
{
	require "/var/www/logreg/_init.php";
	global $email;

	$emailRequest = "SELECT password, IDuser, username, verified FROM user WHERE email = '".$email."';";
	$results = mysqli_query($CONNECTION, $emailRequest);

	if($results == false)
	{
		mysqli_close($CONNECTION);
		return("L_AU_5");
	}

	if(mysqli_num_rows($results) == 1)
    	{
		while($requestLoop = mysqli_fetch_array($results))
        	{		
			if($requestLoop['verified'] == "0")
			{
				mysqli_close($CONNECTION);
				return("L_AU_4");
			}
			$finalResponse = "L_AU_SUCCESSFUL";
			$finalResponse .= $RESTRICTED_MARK.$requestLoop['password'];
               		$finalResponse .= $RESTRICTED_MARK.$requestLoop['IDuser'];
	                $finalResponse .= $RESTRICTED_MARK.$requestLoop['username'];
	                $finalResponse .= $RESTRICTED_MARK.$email;

			mysqli_close($CONNECTION);
			return($finalResponse);
		}
		mysqli_close($CONNECTION);
        	return("L_AU_1");
    	}
    	else if(mysqli_num_rows($results) == 0)
	{
		mysqli_close($CONNECTION);
        	return("L_AU_2");
	}
	else if(mysqli_num_rows($results) > 1)
	{
		mysqli_close($CONNECTION);
		return("L_AU_3");
	}
	mysqli_close($CONNECTION);
    	return("L_AU_2");	
}

?>
