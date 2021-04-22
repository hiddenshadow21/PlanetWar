<?php

function SendAuthenticationMail($username, $email, $code)
{
	require "/var/www/mail/sendMail.php";

	$subject = "PlanetWar - new password verification";

	$body = "<h1 style='text-align:center; color: #6BECF7; font-size:5em;'>PlanetWar</h1>";
	$body .= "<h2 style='text-align:center; color: #6BECF7; font-size:3em;'>Hi ".$username.".</h2>";
	$body .= "<h3 style='text-align:center; font-size:1em;'>Copy code below and paste it in game form!</h3>";
	$body .= "<h3 style='text-align:center; font-size:1em; color: #6BECF7;'>".$code."</h3>";
	$body .= "<h3 style='text-align:center; font-size:1em;'>If you did not want to change Your password, please ignore this message.</h3>";
	
	$response = SendMail($email, $subject, $body);
	
	return($response);
}

?>
