<?php

$email = "";
$password = "";

function DownloadDataFromUnity()
{
	global $email, $password;

	if(isset($_POST["email"]))
	{
		$email = $_POST["email"];
	}
	else
	{
		return("L_DDFU_1");
	}
	
	if(isset($_POST["password"]))
	{
		$password = $_POST["password"];
	}
	else
	{
		return("L_DDFU_2");
	}
	
	return("_SUCCESSFUL");
}

function AuthorizeUser()
{
	require "/var/www/logreg/_init.php";
	global $email, $password;

	$emailRequest = "SELECT passwd, IDuser, username, verified FROM user WHERE email = '".$email."';";
	$results = mysqli_query($CONNECTION, $emailRequest);

	if(mysqli_num_rows($results) == 1)
    	{
		while($requestLoop = mysqli_fetch_array($results))
        	{
			if($requestLoop['passwd'] == $password)
			{
				if($requestLoop['verified'] == "0")
				{
					return("L_AU_4");
				}
				$finalResponse = "L_AU_SUCCESSFUL";
                		$finalResponse .= $RESTRICTED_MARK.$requestLoop['IDuser'];
		                $finalResponse .= $RESTRICTED_MARK.$requestLoop['username'];
		                $finalResponse .= $RESTRICTED_MARK.$email;

				return($finalResponse);
			}
		}
        	return("L_AU_1");
    	}
    	else if(mysqli_num_rows($results) == 0)
    	{
        	return("L_AU_2");
    	}
    	return("L_AU_2");	
}


