if exists(select 1 from sysobjects where name = 'tr_i_client_summit')
drop trigger tr_i_client_summit
go
create trigger "tr_i_client_summit" after update order 2 on
DBA.client
referencing new as new_row
for each row
begin
	if not exists (select 1 from dba.t_summit_data_out where do_type = 3 and do_key = new_row.ccode) then
		insert into DBA.t_summit_data_out (do_id,do_type,do_key,do_crt_time) values (null,3,new_row.ccode,getdate())
	end if
end;

insert t_upgrade_info values (getdate(),'2L7_10_10_(M)_(113)_create_itr_client_summit',null)