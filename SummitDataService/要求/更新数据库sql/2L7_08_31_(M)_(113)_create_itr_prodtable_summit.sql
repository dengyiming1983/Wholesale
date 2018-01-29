if exists(select 1 from sysobjects where name = 'tr_i_prodtable_summit')
drop trigger tr_i_prodtable_summit
go
create trigger "tr_i_prodtable_summit" after update order 2 on
DBA.prodtable
referencing new as new_row
for each row
begin

  if not exists (select 1 from dba.t_summit_data_out where do_type = 1 and do_key = new_row.p_code) then
	begin
		insert into DBA.t_summit_data_out (do_id,do_type,do_key,do_crt_time) values (null,1,new_row.p_code,getdate())
	end;
  end if;
 
end;

insert t_upgrade_info values (getdate(),'2L7_08_31_(M)_(113)_create_itr_prodtable_summit',null)