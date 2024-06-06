**************** READ ME ****************

1. OVERVIEW:

    My apporach to the problem was the following:
       a. Download the respected zip files.
       b. Extract contents.
       c. Load contents into SQL.
       d. Execute the stored procedure.

   There are things to be aware ofin my solution.

   1. I opted to not use Entity Framework in this situation. It really would not have much a performance factor as far as I am concerned.
   2. The structure of my program and method could be re-written to follow a Production envirnment setting as follows:
         2a.   For the download portion of the files, using a HttpClient is completely fine but SFTP is definitely preferred.
      
        *** URL's were supplied so this approach could not be taken. **
      
         2b.  Re-write these methods/functions to be in an API format. As of right now they are not.
      
         2c.  Use ILogger functionality; since I am using a Console, I decided to Write to Console for message logs.

         2d.  On the data load into SQL table things cou have been done to enhance the perforance. All my attempts to try these methods failed.
              *  Spilt .csv's into different sectons. i.e 4 parts. Process each part and have threads do this simulataneously.
              ** Remove all Triggers/Constraints on tables;  opted to not do this since we needed to create a stored procedure as part of problem.
              *** Ensure the clustere dindex is inserting new records at endof table OR move te clustered index to a column that is frequently used
                        for ordering / joining.
              **** Chnge the recoey model of te database to be BULK_LOGGED during the load. Reduce he amount of data being logged in transaction log.
              ***** Use TABLOCK as a query hint or enable the table optio table lock on bulk load, to hold a lock for the duration of the bulk-import operation ad reduce lock contention.
              ****** Specify ORDER BY claue in the bulk insert operation if the target has a clustered index.

      These are the enhancements that I would take, along with others, if this were to be given a Production environment setting in a future time.
