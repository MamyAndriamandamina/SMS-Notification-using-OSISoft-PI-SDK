<h1>Implementation</h1>
<h2>Step1: .Net Framework</h2>
<p>Create a new Console Application (.NetFramework) using Visual Studio.</p>
<p>The version we have used was VS2019 with .NetFramework 4.6.2, Please feel free to change it as per your need.</p>
<p>Copy the program.cs and paste it into your program.cs</p>
<p>Make sure to include the PI reference into your project (pisdk.dll/PISDKCommon.dll/PITimeServer.dll)</p>
<p>If you have properly installed the PI-SDK into your machine, then you should find easily the above.</p>
<p>we have included a Reference folder in case you might need it.</p>
<p>Please publish your application into your desired location.</p>
<p>The user account executing this application should have access on OSISoft PI point reading database, please contact your PI System Administrator if it is not your case.</p>

<h2>Step2: SQL Database</h2>
<h4>Database</h4>
<p>Create a database into an SQL server instance, please name it as you like, in our case, we have named it "SMS"</p>
<h4>Tables</h4>
<p>Right Click on this created Database name, and click on New Query then copy and paste these SQL Queries with the following order</p>
<p>1st:Run PIDistributionGroup.sql to create the table of users group. Basically, we have used this group to store the name of an existing department within your company. GroupID should be unique. It's a primary key.</p>
<p>2nd: Run PIDeliveryListUsers.sql to create the table of users list. Please specify the name of the recipient list as well as their respective cell phone numbers, their groupID should be part of groupID listed in the created PIDistributionGroup table. It's a foreign key.</p>
<p>3rd: Run PIPointList.sql to create the table of PI point list. Please specify the list of pi point names, these points should be already available in the PI System before you add it, please indicate the Operator such as > to say more than, = to say equal, and finally specify the trigger condition. example1: SINUSOID > 0.5 example2: 32HVAC01 = on</p>
<p>4th: Run PIPointSnapshot.sql to create the table of pi point snapshot list. 
<p>This table will enable you to see the snapshot value of your pi point. This table is fed from your application not from your action. just leave it once created</p>
<p> In normal operation, data found in this table should be exactly equal to the snapashot value of your pi point found in your PI System, it is fed from your sdk application.</p>
<p>5th: Run PISmsQueueList.sql to create the table of sms status. This table will enable you to see whether an sms is sent or ready to be sent. 
<p>This table is partially fed from your application, once created, you have to initialize the SMS Status column by updating it using the last step</p>
<p>It is a mapping of data where you will see the points, the values, the trigger conditions, the users, their respective cell phone numbers and finally the status of their sms: ready/sent.</p>
<h4>Views</h4>
<p>Create View by running SMS_COMBO_1.sql, it will be used in view SMS_COMBO_2.</p>
<p>Create View by running SMS_COMBO_2.sql. This view is used during the execution of our stored procedure to Generate SMS Queue List.</p>
<h4>Stored Procedures</h4>
<p>Create the 3 stored procedures by executing the following SQL queries</p>
<p>Run GenerateSMSQueueList.sql used to update the PISmsQueueList table</p>
<p>Run MergeTable.sql used to update the PIPointSnapshot table</p>
<p>Run ReadSMSTable.sql used to read the list of pi point name</p>

<h2>Step3:Modem</h2>
<p>Installing a modem hardware on a dedicated server is mandatory to allow sending/receiving our sms.</p>
<p>Assuming it is installed in our server with all configuration done as per the vendor of your product: port number, speed...</p>
<p>The next step is to make a test of your modem. </p>
<p>Please try to send an sms manually using AT command from the terminal and make sure the sms is sent properly.</p>
<p>Please download and install an HyperTerminal for testing your modem.</p>
<p>You should receive OK signal before you move to the next step.</p>
<p>For our case, we have used a Wireless Sierra Modem, but you can also use those old mobile cell phone connected on usb port, Nokia...</p>

<h2>Step4: Scheduled Task</h2>
<p>Please feel free to Run a scheduled task as per your need.</p>
<p>In our case, we have created 2 windows tasks scheduler.</p>
<p>You can run it on a SQL server agent as two new jobs.</p>
<p>The 1st task will update the SQL Table.</p> 
<p>Task scheduler1: run UpdateSMSTable.vbs </p>
<p>The 2nd task will check and send the SMS according to the table reading value.</p> 
<p>Task scheduler2: run SendSMSTable.vbs </p>
<p>Please configure the two scheduled tasks with a minimum value of about 1 minute time interval between the 1st task and the 2nd.</p>

<h2>Final Step: Update the SMS Status in PISmsQueueList Table</h2>
<p>After having run the 2 scheduled task from the first time, please check the status of your PISmsQueueList Table table</p>
<p>The default value of SMS Status Column in PISmsQueueList table was not set and will return NULL after having started this application.</p>
<p>You then have to update this column for the first time and ONLY after having added new tags or new user list.</p>
<p>Please initialize the PISmsQueueList table with a value of "ready" on all SMS_Status column to allow the sms to start.</p>
<p>The next operation does not require any human action, the application will toggle between "ready" and "sent".</p>

<h1>Operation</h1>
<h2>Starting</h2>
<p>In normal operation, this process will run endlessly with no human action required.</p>
<p>To run this application, you just need to schedule and run the 2 vbs applications, they will  invoke 2 main method in our .netframework application.</p>
<p>Please consider about 1 minute time interval between them.</p>
<h2>Monitoring</h2>
<p>Please Check your PIPointSnapshot table to see the latest value in PI System,</p>
<p>Please Check your PISmsQueueList table to status of your notification,</p>
<p>That's it! good luck!</p>




