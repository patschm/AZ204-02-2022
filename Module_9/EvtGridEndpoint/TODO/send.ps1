#First: Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Scope CurrentUser
$rsname = "az204"
$topic = "ps-mytopic"

$endpoint = (Get-AzEventGridTopic -ResourceGroupName $rsname -Name $topic).Endpoint
$keys = Get-AzEventGridTopicKey -ResourceGroupName $rsname -Name $topic

$eventID = Get-Random 99999
$eventDate = Get-Date -Format s
$htbody = @{
    id= $eventID
    eventType="inserted"
    subject="grid/post"
    eventTime= $eventDate   
    data= @{
        een="42"
        twee="Hello Event"
    }
    dataVersion="1.0"
}
$body = "["+(ConvertTo-Json $htbody)+"]"


Invoke-WebRequest -Uri $endpoint -Method POST -Body $body -Headers @{"aeg-sas-key" = $keys.Key1} -UseBasicParsing