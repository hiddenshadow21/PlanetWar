<?php

$email = "";
$code = "";
$username = "";

function DownloadDataFromUnity()
{
	global $email;

	if(isset($_POST["email"]))
	{
		$email = $_POST["email"];
	}
	else
	{
		return("NPEV_DDFU_1");
	}
	
	return("_SUCCESSFUL");
}

function SetCode()
{
	require "/var/www/logreg/_init.php";
	global $email, $code, $username;

	$emailRequest = "SELECT email, username, verified FROM user WHERE email = '".$email."';";
	$results = mysqli_query($CONNECTION, $emailRequest);

	if($results == false)
	{
		mysqli_close($CONNECTION);
		return("NPEV_SC_1");
	}

	if(mysqli_num_rows($results) == 1)
	{
		while($requestLoop = mysqli_fetch_array($results))
		{
			if($requestLoop['verified'] == "0")
			{
				mysqli_close($CONNECTION);
				return("NPEV_SC_4");
			}

			$username = $requestLoop['username'];
			$code = md5(rand(0, 1000));
			$inputQuery = "UPDATE user SET code = '".$code."' WHERE email = '".$email."';";

			if(mysqli_query($CONNECTION, $inputQuery)) 
			{
				mysqli_close($CONNECTION);
				return("_SUCCESSFUL");
			}	

			mysqli_close($CONNECTION);
			return("NPEV_SC_3"); 
		}
	} 
	mysqli_close($CONNECTION);
	return("NPEV_SC_2");
}

function SendVerificationMail()
{
	require "/var/www/logreg/_codeAuthentication.php";
	global $username, $email, $code;

	$response = SendAuthenticationMail($username, $email, $code);
				
	if($response != "SM_SM_SUCCESSFUL_1")
	{
		return($response);
	}
	return("NPEV_SVM_SUCCESSFUL");		
}

?>
